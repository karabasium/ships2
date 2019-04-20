using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("game started");
		Unit u = new Unit("brig", Vector2.zero, 1);
		Field f = new Field(3, 4);
		f.AddShip(1, 2, u);
		FieldAppearance fa = new FieldAppearance(f);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
