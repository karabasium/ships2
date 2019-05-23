using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
	PLAYER_1,
	PLAYER_2,
	NONE
}

public enum Weather_type
{
	WIND,
	STORM,
	CALM
}

public enum Action
{
	MOVE,
	FIRE,
	DRIFT,
	ANY
}

public class GameController : MonoBehaviour {
	public static GameController instance;
	private GameObject fieldObject;
	public Weather currentWeather;
	public FieldAppearance fa;
	private WeatherAppearance wa;
	private Field f;
	private ClickEventsController clickEventsController;
	public Player currentPlayer;
	private bool gameOver;
	private HUD hud;




	void Awake()
	{
		Debug.Log("Game Controller Awaker");
		MakeSingleton();

		currentPlayer = Player.PLAYER_1;
		gameOver = false;

		f = new Field(12, 16);

		Unit u1 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1 );
		Unit u2 = new Unit("brig", Vector2Int.zero, Player.PLAYER_1);

		Unit u3 = new Unit("brig", Vector2Int.zero, Player.PLAYER_2 );
		Unit u4 = new Unit("brig", Vector2Int.zero, Player.PLAYER_2);

		f.AddUnit(new Vector2Int(3, 3), u1);
		//f.AddUnit(new Vector2Int(4, 6), u2);
		f.AddUnit(new Vector2Int(4, 5), u3);
		//f.AddUnit(new Vector2Int(3, 7), u4);

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();

		//wa = new WeatherAppearance(currentWeather);
		//wa.UpdateWeatherAppearance();
	}

	void Start()
	{
		Debug.Log("Game Controller Start");
		fa.Init(f);
		clickEventsController.Init(fa, f);
		f.SelectRandomUnit(currentPlayer);
		fa.UpdateField();

		hud = GameObject.Find("HUD").GetComponent<HUD>();
		hud.Init(f, currentWeather);
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
		if(currentWeather.needPerformStormActions)
		{
			f.ReleaseUnitsSelection();
			f.StormMoveAllShips();
			f.unitsAnimationInProgress = true;
			currentWeather.needPerformStormActions = false;
		}
		if (f.unitsAnimationInProgress)
		{
			bool animationFinished = true;
			foreach (Unit u in f.GetUnits())
			{
				if (u.unitNeedsMovementAnimation)
				{
					animationFinished = false;
				}
			}
			if (animationFinished)
			{
				f.unitsAnimationInProgress = false;
				f.SelectRandomUnit(currentPlayer);
				fa.UpdateField();
			}
		}
	}

	private bool WaitUntilMovementAnimationEnds()
	{
		foreach (Unit u in f.GetUnits())
		{
			if (u.unitNeedsMovementAnimation)
			{		
				return false;
			}
		}
		return true;
	}

	public void SetNextPlayerAsActive()
	{
		if (currentPlayer == Player.PLAYER_1)
		{
			currentPlayer = Player.PLAYER_2;			
		}
		else
		{
			currentPlayer = Player.PLAYER_1;
		}
		currentWeather.RefreshWeather();
		currentWeather.needHUDUpdate = true;
		f.ReleaseUnitsSelection();
		f.RefreshPlayerUnits(currentPlayer);
		f.SelectRandomUnit(currentPlayer);
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
