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
	private GameObject fieldObject;
	public Weather currentWeather;
	private FieldAppearance fa;
	private Field f;
	private ClickEventsController clickEventsController;




	void Awake()
	{
		f = new Field(8, 9);

		Unit u1 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1 );
		Unit u2 = new Unit("brig", Vector2Int.zero, Player.PLAYER_2 );

		f.AddUnit(new Vector2Int(0, 0), u1);
		f.AddUnit(new Vector2Int(3, 4), u2);

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
	}
	void Start () {
		fa.Init( f );
		clickEventsController.Init( fa, f);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
