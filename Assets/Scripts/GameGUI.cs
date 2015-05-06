using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if (GUI.Button (new Rect (0f, 0f, 50f, 10f), "Restart")) {
			Grid.instance.RefreshGrid();
		}
	}
}
