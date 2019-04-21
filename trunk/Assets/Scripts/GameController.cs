using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	GameObject fieldObject;
	Field f;

	void Awake()
	{
		Unit u1 = new Unit("brig", Vector2.zero, 1);
		Unit u2 = new Unit("brig", Vector2.zero, 1);
		Unit u3 = new Unit("brig", Vector2.zero, 1);
		f = new Field(3, 4);
		f.AddShip(0, 3, u1);
		//f.AddShip(1, 2, u2);
		//f.AddShip(0, 0, u3);
		fieldObject = new GameObject();
		fieldObject.AddComponent<FieldAppearance>();
	}
	void Start () {
		//Debug.Log("game started");


		//FieldAppearance.instance.SetField(f);
		fieldObject.GetComponent<FieldAppearance>().SetField(f);
		//FieldAppearance fa = new FieldAppearance(f);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
