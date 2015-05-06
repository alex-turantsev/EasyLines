using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
	public int xSize, ySize;
	public float itemWidth = 1f;
	private GameObject[] _items;
	private GridItem[,] _gridItems;
	private GridItem _currentlySelected;

	public static List<GridItem> selectedItems = new List<GridItem>();
	public static bool selectedMode = false;
	public static int selectedId;

	private void Start () {
		GetItems ();
		FillGrid ();
		GridItem.OnMouseDownItemEventHandler += OnMouseDownItem;
		GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
	}
	
	private void FillGrid(){
		_gridItems = new GridItem[xSize, ySize];
		for (var x = 0; x < xSize; x++) {
			for(var y = 0; y < ySize; y++){
				_gridItems[x,y] = InstantieItem(x,y);
			}
		}
	}

	private void Update(){
		if (Input.GetMouseButtonUp (0)) {
			//OnMouseUp();
		}
	}
	
	public void DeselectAll(){
		foreach (var g in _gridItems) {
			g.SetInactive(false);
		}
		for (var i = 0; i < selectedItems.Count; i++) {
			selectedItems[i].Selected(false);
		}
		selectedItems.Clear ();
	}

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
			DeselectAll();

			return;
		}
	}

	private void OnMouseOverItem(GridItem item){
		if (!selectedMode || item.inactive)
			return;
		var itemS = selectedItems.Last ();
		if (itemS == item)
			return;
		if (GridItem.isNearestItems (item, itemS)) {
			item.Selected(true);
		}
	}

	private GridItem InstantieItem(int x, int y){
		var item = _items [Random.Range (0, _items.Length)];
		var newItem = Instantiate (item, new Vector3 (x * itemWidth, y), Quaternion.identity) as GameObject;
		var gridItem = newItem.GetComponent<GridItem> ();
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
