using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid instance;
	public int xSize, ySize;
	public float itemWidth = 1f;
	private GameObject[] _items;
	private GridItem[,] _gridItems;
	private GridItem _currentlySelected;
	private int score = 0;

	public static List<GridItem> selectedItems = new List<GridItem>();
	public static bool selectedMode = false;
	public static int selectedId;
	public static Transform itemsParent;

	private void Start () {
		var go = GameObject.Find ("GridGameObject");
		if (go == null) {
			Debug.LogError("It must be GridGameObject");
			return;
		}
		itemsParent = go.transform;
		itemsParent.transform.position = new Vector3 ();
		instance = this;
		GetItems ();
		FillGrid ();
		GridItem.OnMouseDownItemEventHandler += OnMouseDownItem;
		GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
	}
	
	private void FillGrid(){
		itemsParent.transform.position = new Vector3 ((xSize*itemWidth)/-2, 0f);
		_gridItems = new GridItem[xSize, ySize];
		for (var x = 0; x < xSize; x++) {
			for(var y = 0; y < ySize; y++){
				_gridItems[x,y] = InstantieItem(x,y);
			}
		}
	}
	#region Score
	public void RefreshScore(){
		score = 0;
		GameGUI.instance.scoreCount.text = "0";
	}

	public void AddScore (int score){
		this.score += score;
		GameGUI.instance.scoreCount.text = this.score.ToString();
	}
	#endregion Score

	public void RefreshGrid(){
		score = 0;
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void DeselectAll(){
		foreach (var g in _gridItems) {
			g.SetInactive(false);
		}
		for (var i = 0; i < selectedItems.Count; i++) {
		}
		foreach (var g in selectedItems) {
			g.Selected(false);
		}
		selectedItems.Clear ();
	}

	#region MouseEvents
	private void OnMouseDownItem(GridItem item){
		if (!selectedMode) {
			foreach (var g in _gridItems) {
				if (g.id != item.id)
					g.SetInactive (true);
			}
			selectedMode = true;
			selectedId = item.id;
			return;
		} else {
			if(item.id != selectedId || selectedItems.Count < 4){
				selectedMode = false;
				DeselectAll();
				return;
			}
			//if items id is right and line consist of 4 and more items, destroy them
			selectedMode = false;
			for(var i = selectedItems.Count - 1; i >= 0; i--){
				var gridItem = selectedItems[i];
				selectedItems.RemoveAt(i);
				_gridItems[gridItem.x,gridItem.y] = null;
				Destroy(gridItem.gameObject);
			}
			FillEmptinessAndGenerateNew();
			//DeselectAll();

			return;
		}
	}

	private void OnMouseOverItem(GridItem item){
		if (!selectedMode || item.inactive)
			return;
		var lastItem = selectedItems.Last ();
		if (item == lastItem)
			return;
		if (!item.selected) {
			if (GridItem.isNearestItems (item, lastItem)) {
				item.Selected (true);
			}
		} else {
			var pos = selectedItems.IndexOf(item);
			if(selectedItems.Count > (pos + 1)){
				pos++;
				var count = selectedItems.Count - pos;
				var list = selectedItems.GetRange(pos, count);
				foreach(var g in list){
					g.Selected(false);
				}
				selectedItems.RemoveRange(pos, count);
			}
		}
	}
	#endregion MouseEvents

	//todo check when @null item null@
	private void FillEmptinessAndGenerateNew(){
		for (var x = 0; x < xSize; x++) {
			int y = 0;
			while(y < ySize){
				if(_gridItems[x,y] != null){
					y++;
				} else {
					var y1 = y;	//y - first null 
					while(y1 < ySize && _gridItems[x,y1] == null){
						y1++;
					}
					if(y1 < ySize){
						var y2 = y1; // y1 - first item
						while(y2 < ySize && _gridItems[x,y2] != null){
							y2++; // last item in set
						}
						if(y2 >= ySize || _gridItems[x,y2] == null)
							y2--;
						/*for(int i = y, i1 =0; (i < y1) || (i1 < (y2- y1)); i++, i1++){
							Debug.Log (x+" "+i +" change on "+x+" "+(y1+i1));
							_gridItems[x,i] = _gridItems[x,y1+i1];
							_gridItems[x,i].OnItemPositionChanged(x,i);
						}*/
						Debug.Log (y+" "+y1+" "+y2);
						for(var i1 =0; i1 < (y2- y1+1); i1++){
							Debug.Log (x+" "+(i1+y) +" change on "+x+" "+(y1+i1));
							_gridItems[x,i1+y] = _gridItems[x,y1+i1];
							_gridItems[x,i1+y].OnItemPositionChanged(x,i1+y);
						}
						y = y1;	
					}
				}
			}
		}
	}

	private GridItem InstantieItem(int x, int y){
		var item = _items [Random.Range (0, _items.Length)];
		var newItem = Instantiate (item, Vector3.zero, Quaternion.identity) as GameObject;
		var gridItem = newItem.GetComponent<GridItem> ();
		gridItem.transform.parent = itemsParent;
		gridItem.transform.localPosition = new Vector3 (x * itemWidth, y);
		gridItem.OnItemPositionChanged (x, y);
		return gridItem;
	}

	private void GetItems(){
		_items = Resources.LoadAll<GameObject>("Prefabs");
		for (var i = 0; i < _items.Length; i++) {
			_items[i].GetComponent<GridItem>().id = i;
		}
	}
}
