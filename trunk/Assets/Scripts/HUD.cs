using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public Text w_text;

	// Use this for initialization
	void Start () {
		Debug.Log("HUD start");
		w_text = GameObject.Find("WeatherValue").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void NextTurn()
	{
		Debug.Log(GameController.instance);
		GameController.instance.SetNextPlayerAsActive();
		Debug.Log("Next turn");
	}
}
