using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FastShowScores : MonoBehaviour {
	[HideInInspector]
	public Text uitext;

	public float speed = 5f;

	private Color color;
	private float alpha;

	void Start () {
		uitext = GetComponent<Text> ();
		color = uitext.color;
	}

	void OnEnable(){
		if (uitext == null) {
			Start();
		}
		uitext.color = color;
		alpha = color.a;
	}

	void Update () {
		if (alpha <= 0) {
			gameObject.SetActive(false);
		}
		alpha -= speed * Time.deltaTime;
		uitext.color = new Color(color.r, color.g, color.b, alpha);
	}
}
