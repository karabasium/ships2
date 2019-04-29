using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public static GameController instance;
	GameObject fieldObject;
	public Weather currentWeather;
	Field f;

	void Awake()
	{
		Unit u1 = new Unit("brig", Vector2.zero, 1);
		Unit u2 = new Unit("brig", Vector2.zero, 1);
		Unit u3 = new Unit("brig", Vector2.zero, 1);
		f = new Field(10, 12);
		f.AddUnit(0, 0, u1);
		f.AddUnit(0, 0, u2);
		//f.AddShip(0, 0, u3);
		fieldObject = new GameObject();
		fieldObject.AddComponent<FieldAppearance>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
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
