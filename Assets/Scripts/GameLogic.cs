using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {
	public static GameLogic instance;

	public int TurnsCount = 5;

	[HideInInspector]
	public int score;

	private int turnsTemp;
	
	void Awake () {
		instance = this;
		GridItem.ready = true;
		turnsTemp = TurnsCount;
	}

	void Start(){
		Restart ();
	}

	public static void RefreshScoreAndTurns(){
		instance.score = 0;
		GameGUI.instance.scoreCountText.text = "0";
		instance.TurnsCount = instance.turnsTemp;
		GameGUI.instance.turnLeftText.text = instance.TurnsCount.ToString ();
	}
	
	public static void AddScore (int score, Vector3 position){
		instance.score += score;
		GameGUI.instance.scoreCountText.text = instance.score.ToString();
		GameGUI.instance.SetPositionOfOnesScores (score, position);
		instance.TurnsCount--;
		GameGUI.instance.turnLeftText.text = instance.TurnsCount.ToString ();
		if (instance.TurnsCount <= 0) {
			GridItem.ready = false;
			GameGUI.instance.OnGameEnd();
		}
	}

	public static void Restart(){
		RefreshScoreAndTurns ();
		if (Grid.instance != null)
			Grid.instance.RefreshGrid ();
		GridItem.ready = true;
	}
}
