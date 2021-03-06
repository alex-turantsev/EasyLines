﻿using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
	public static bool ready = true;
	public int x {
		get;
		private set;
	}

	public int y {
		get;
		private set;
	}
	[HideInInspector]
	public bool selected = false;
	[HideInInspector]
	public bool inactive = false;
	[HideInInspector]
	public int id;
	[HideInInspector]
	public new Rigidbody2D rigidbody;
	[HideInInspector]
	public new Transform transform;
	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	public GameObject selectedSprite;

	public delegate void OnMouseDownItem(GridItem item);
	public static event OnMouseDownItem OnMouseDownItemEventHandler;

	public delegate void OnMouseOverItem(GridItem item);
	public static event OnMouseOverItem OnMouseOverItemEventHandler;

	private void Awake(){
		this.rigidbody = gameObject.GetComponent<Rigidbody2D> ();
		this.transform = gameObject.GetComponent<Transform> ();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
	}
	private void Start(){
		selectedSprite.SetActive (false);
	}

	/// <summary>
	/// set indexes in grid and name
	/// </summary>
	public void OnItemPositionChanged(int newx, int newy){
		x = newx;
		y = newy;
		gameObject.name = string.Format ("Item[{0}][{1}]", newx, newy);
	}

	private void OnMouseDown(){
		if (!ready)
			return;
		SetSelected (true);
		if (OnMouseDownItemEventHandler != null) {
			OnMouseDownItemEventHandler(this);
		}
	}

	private void OnMouseOver(){
		if (!ready)
			return;
		if (OnMouseOverItemEventHandler != null)
			OnMouseOverItemEventHandler (this);
	}

	public void SetInactive(bool inactive){
		this.inactive = inactive;
		var c = spriteRenderer.color;
		var opacity = inactive ? 0.2f : 1f;
		spriteRenderer.color = new Color (c.r, c.g, c.b, opacity);
	}

	public void SetSelected(bool selected){
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
