using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
	/*public int x {
		get;
		private set;
	}

	public int y {
		get;
		private set;
	}*/
	public int x;
	public int y;
	public bool selected = false;
	public bool inactive = false;

	public int id;
	public Rigidbody2D rigid;
	public SpriteRenderer spriteRenderer;
	public GameObject selectedSprite;

	public delegate void OnMouseDownItem(GridItem item);
	public static event OnMouseDownItem OnMouseDownItemEventHandler;

	public delegate void OnMouseOverItem(GridItem item);
	public static event OnMouseOverItem OnMouseOverItemEventHandler;

	private void Start(){
		rigid = gameObject.GetComponent<Rigidbody2D> ();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		selectedSprite.SetActive (false);
	}

	public void OnItemPositionChanged(int newx, int newy){
		x = newx;
		y = newy;
		gameObject.name = string.Format ("Item[{0}][{1}]", newx, newy);
	}

	private void OnMouseDown(){
		Selected (true);
		if (OnMouseDownItemEventHandler != null) {
			OnMouseDownItemEventHandler(this);
		}
	}

	private void OnMouseOver(){
		if (OnMouseOverItemEventHandler != null)
			OnMouseOverItemEventHandler (this);
	}

	public void SetInactive(bool inactive){
		this.inactive = inactive;
		var c = spriteRenderer.color;
		var opacity = inactive ? 0.2f : 1f;
		spriteRenderer.color = new Color (c.r, c.g, c.b, opacity);
	}

	public void Selected(bool selected){
		if (this.selected != selected) {
			selectedSprite.SetActive (selected);
			this.selected = selected;
			if(selected)
				Grid.selectedItems.Add (this);
		}
	}

	public static bool isNearestItems(GridItem a, GridItem b){
		var max = Mathf.Max (Mathf.Abs (a.x - b.x), Mathf.Abs (a.y - b.y));
		if (max == 1)
			return true;
		return false;

	}
}
