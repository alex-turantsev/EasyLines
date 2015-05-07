using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour {
	public static GameGUI instance;

	//common scores
	public Text scoreCountText;
	//scores for one action
	public FastShowScores scoreCountOnes;

	public Text turnLeftText;

	public GameObject endGamePanelObject;
	public Text endGameScoreText;

	private RectTransform canvasTransform;

	void Awake(){
		instance = this;
	}

	void Start () {
		canvasTransform = gameObject.GetComponent<RectTransform> ();
		scoreCountOnes.gameObject.SetActive (false);
		endGamePanelObject.SetActive (false);
	}

	public void SetPositionOfOnesScores(int scores, Vector3 worldCoordinates){
		scoreCountOnes.gameObject.SetActive (true);
		var viewportPosition = Camera.main.WorldToViewportPoint (worldCoordinates);
		var anchoredPosition = new Vector2(
			((viewportPosition.x*canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.37f)),
			((viewportPosition.y*canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));
		
		scoreCountOnes.uitext.rectTransform.anchoredPosition = anchoredPosition;
		scoreCountOnes.uitext.text = scores.ToString();
	}

	public void OnRestart(){
		endGamePanelObject.SetActive (false);
		GameLogic.Restart ();
	}

	public void OnGameEnd(){
		endGameScoreText.text = scoreCountText.text;
		endGamePanelObject.SetActive (true);
	}

}
