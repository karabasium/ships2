using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
	PLAYER_1,
	PLAYER_2,
	NONE
}

public enum weather_type
{
	WIND,
	STORM,
	CALM
}

public class GameController : MonoBehaviour {
	public static GameController instance;
	private GameObject fieldObject;
	public Weather currentWeather;
	private FieldAppearance fa;
	private WeatherAppearance wa;
	private Field f;
	private ClickEventsController clickEventsController;
	private Player currentPlayer;
	private bool gameOver;




	void Awake()
	{
		MakeSingleton();

		currentPlayer = Player.PLAYER_1;
		gameOver = false;

		f = new Field(12, 16);

		Unit u1 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1 );
		Unit u2 = new Unit("brig", Vector2Int.zero, Player.PLAYER_2 );

		f.AddUnit(new Vector2Int(4, 5), u1);
		f.AddUnit(new Vector2Int(6, 7), u2);

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();

		wa = new WeatherAppearance(currentWeather);
		wa.UpdateWeatherAppearance();
	}

	void Start()
	{
		fa.Init(f);
		clickEventsController.Init(fa, f);

	}


	void MakeSingleton()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}




	void Update () {
		if (!gameOver)
		{
			if (WhoIsWinner() == Player.NONE)
			{
				if (PlayerDidAnyPossibleActions())
				{
					SetNextPlayerAsActive();
				}
			}
			else
			{
				gameOver = true;
				Debug.Log("GAME OVER! Winner is player " + WhoIsWinner().ToString());
			}
		}
	}

	public void SetNextPlayerAsActive()
	{
		Debug.Log("SetNextPlayerAsActive");
		if (currentPlayer == Player.PLAYER_1)
		{
			currentPlayer = Player.PLAYER_2;			
		}
		else
		{
			currentPlayer = Player.PLAYER_1;
		}
		currentWeather.RefreshWeather();
		wa.UpdateWeatherAppearance();
		f.ReleaseUnitsSelection();
		f.RefreshPlayerUnits(currentPlayer);
		fa.UpdateField();
		Debug.Log("Current player = " + currentPlayer.ToString());
	}

	private bool PlayerDidAnyPossibleActions()
	{
		foreach (Unit u in f.GetUnits())
		{
			if (u.player == currentPlayer)
			{
				if (!u.fireDone || !u.movementDone)
				{
					return false;
				}
			}
		}
		return true;
	}

	private Player WhoIsWinner()
	{
		if (f.GetAlivePlayerUnits( Player.PLAYER_1 ).Count == 0)
		{ 
			return Player.PLAYER_2;			
		}
		else if (f.GetAlivePlayerUnits( Player.PLAYER_2).Count == 0)
		{
			return Player.PLAYER_1;
		}
		return Player.NONE;
	}
}
