using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
	PLAYER_1,
	PLAYER_2
}

public class GameController : MonoBehaviour {
	public static GameController instance;
	GameObject fieldObject;
	public Weather currentWeather;
	private FieldAppearance fa;
	Field f;




	void Awake()
	{
		f = new Field(8, 9);

		Unit u1 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1 );
		Unit u2 = new Unit("brig", Vector2Int.zero, Player.PLAYER_2 );
		Unit u3 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1 );

		f.AddUnit(new Vector2Int(0, 0), u1);
		f.AddUnit(new Vector2Int(3, 4), u2);
		//f.AddShip(0, 0, u3);
		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
	}
	void Start () {
		////Debug.Log("game started");


		//FieldAppearance.instance.SetField(f);
		fa.Init( f );
		//FieldAppearance fa = new FieldAppearance(f);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
