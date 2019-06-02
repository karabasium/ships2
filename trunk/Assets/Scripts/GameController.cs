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
	NONE,
	ANY
}

public enum CellType
{
	SEA,
	LAND
}

public enum GAME_STATE
{
	INITIALIZATION,
	NOTHING_HAPPENS,
	ANIMATION_IN_PROGRESS,
	FIELD_APPEARANCE_NEED_TO_UPDATE
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
	public GAME_STATE gameState;
	public readonly float HIT_PROBABILITY = 1.0f;




	void Awake()
	{
		gameState = GAME_STATE.INITIALIZATION;
		MakeSingleton();

		currentPlayer = Player.PLAYER_1;
		gameOver = false;

		f = new Field(12, 16);
		List<Cell> cells = f.GetAllCells();
		cells[1].CellType = CellType.LAND;
		cells[2].CellType = CellType.LAND;
		cells[3].CellType = CellType.LAND;
		cells[4].CellType = CellType.LAND;
		cells[5].CellType = CellType.LAND;

		Unit u1 = new Unit("brig", Player.PLAYER_1 );
		Unit u2 = new Unit("brig", Player.PLAYER_1);

		Unit u3 = new Unit("brig", Player.PLAYER_2 );
		Unit u4 = new Unit("brig", Player.PLAYER_2);

		f.AddUnit(new Vector2Int(1, 1), u1);
		f.AddUnit(new Vector2Int(1, 3), u2);
		f.AddUnit(new Vector2Int(4, 5), u3);
		f.AddUnit(new Vector2Int(3, 7), u4);

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();
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
			currentWeather.needPerformStormActions = false;
		}
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
			if (u.Player == currentPlayer)
			{
				if (!u.FireDone || !u.MovementDone)
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
