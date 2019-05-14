using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("HUD start");
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
