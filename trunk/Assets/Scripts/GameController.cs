using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("game started");
		Unit u1 = new Unit("brig", Vector2.zero, 1);
		Unit u2 = new Unit("brig", Vector2.zero, 1);
		Unit u3 = new Unit("brig", Vector2.zero, 1);
		Field f = new Field(3, 4);
		f.AddShip(1, 2, u1);
		f.AddShip(1, 2, u2);
		f.AddShip(1, 1, u3);
		FieldAppearance fa = new FieldAppearance(f);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
