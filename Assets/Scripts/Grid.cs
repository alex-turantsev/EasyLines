using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid instance;

	public int xSize, ySize;
	public float itemWidth = 1f;

	private GameObject[] _itemsPrefabs;
	private GridItem[,] _gridItems;

	public static List<GridItem> selectedItems = new List<GridItem>();
	public static bool selectedMode = false;
	public static int selectedId;
	public static Transform itemsParent;

	private void Awake(){
		instance = this;
		var go = GameObject.Find ("ParentForItems");
		if (go == null) {
			Debug.LogError("It must be ParentForItems");
			return;
		}
		itemsParent = go.transform;
		GetItems ();
	}

	private void Start () {
		GridItem.OnMouseDownItemEventHandler += OnMouseDownItem;
		GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
	}

	private void FillGrid(){
		itemsParent.transform.position = new Vector3 (((xSize-1)*itemWidth)/-2, 0f);
		_gridItems = new GridItem[xSize, ySize];
		for (var x = 0; x < xSize; x++) {
			for(var y = 0; y < ySize; y++){
				_gridItems[x,y] = InstantieItem(x,y);
			}
		}
	}

	public void RefreshGrid(){
		if(_gridItems != null)
			foreach (var item in _gridItems) {
				Destroy(item.gameObject);
			}
		FillGrid ();
	}
	
	public void DeselectAll(){
		foreach (var g in _gridItems) {
			g.SetInactive(false);
		}
		for (var i = 0; i < selectedItems.Count; i++) {
		}
		foreach (var g in selectedItems) {
			g.SetSelected(false);
		}
		selectedItems.Clear ();
	}

	#region MouseEvents
	private void OnMouseDownItem(GridItem item){
		if (!selectedMode) {
			//if first click on item
			foreach (var g in _gridItems) {
				if (g.id != item.id)
					g.SetInactive (true);
			}
			selectedMode = true;
			selectedId = item.id;
			return;
		} else {
			//second click on some item
			if(item.id != selectedId || selectedItems.Count < 4){
				selectedMode = false;
				DeselectAll();
				return;
			}
			//if items id is right and line consist of 4 and more items, destroy them
			selectedMode = false;
			float score = selectedItems.Count * Mathf.Pow(((float)(selectedItems.Count)/12)+1f, 2f);
			var position = selectedItems[selectedItems.Count/2].transform.position;
			GameLogic.AddScore((int)score, position);

			//Destroing chain
			for(var i = selectedItems.Count - 1; i >= 0; i--){
				var gridItem = selectedItems[i];
				selectedItems.RemoveAt(i);
				_gridItems[gridItem.x,gridItem.y] = null;
				Destroy(gridItem.gameObject);
			}
			StartCoroutine(FillEmptinessAndGenerateNew());
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
			//if item near last. select him
			if (GridItem.isNearestItems (item, lastItem)) {
				item.SetSelected (true);
			}
		} else {
			//if hover item that not last and selected
			var pos = selectedItems.IndexOf(item);
			if(selectedItems.Count > (pos + 1)){
				pos++;
				var count = selectedItems.Count - pos;
				var list = selectedItems.GetRange(pos, count);
				foreach(var g in list){
					g.SetSelected(false);
				}
				selectedItems.RemoveRange(pos, count);
			}
		}
	}
	#endregion MouseEvents

	/// <summary>
	/// Correction of indexes and generation new items
	/// </summary>
	private IEnumerator FillEmptinessAndGenerateNew(){
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
					if(y1 >= ySize){
						y = ySize;
					}else{
						var y2 = y1; // y1 - first item
						while(y2 < ySize && _gridItems[x,y2] != null){
							y2++; // y2 - last item in set
						}
						if(y2 >= ySize || _gridItems[x,y2] == null)
							y2--;
						//remove "emptyness" between not null items
						for(var i1 =0; i1 < (y2- y1+1); i1++){
							_gridItems[x,i1+y] = _gridItems[x,y1+i1];
							_gridItems[x,y1+i1] = null;
							_gridItems[x,i1+y].OnItemPositionChanged(x,i1+y);
						}
						y = 0;
					}
				}
			}
			//set active all items in the grid
			for(y = 0; y < ySize; y++){
				if(_gridItems[x,y] != null){
					_gridItems[x,y].SetInactive(false);
				}
			}
		}
		yield return new WaitForSeconds (0.4f);
		//generate new items
		for (var x = 0; x < xSize; x++) {
			for(var y = 0; y < ySize; y++){
				if(_gridItems[x,y] == null)
					_gridItems[x,y] = InstantieItem(x,y);
			}
		}
	}

	private GridItem InstantieItem(int x, int y){
		var item = _itemsPrefabs [Random.Range (0, _itemsPrefabs.Length)];
		var newItem = Instantiate (item, Vector3.zero, Quaternion.identity) as GameObject;
		var gridItem = newItem.GetComponent<GridItem> ();
		gridItem.transform.parent = itemsParent;
		gridItem.transform.localPosition = new Vector3 (x * itemWidth, y);
		gridItem.OnItemPositionChanged (x, y);
		return gridItem;
	}
	
	private void GetItems(){
		_itemsPrefabs = Resources.LoadAll<GameObject>("Prefabs");
		for (var i = 0; i < _itemsPrefabs.Length; i++) {
			_itemsPrefabs[i].GetComponent<GridItem>().id = i;
		}
	}
}
