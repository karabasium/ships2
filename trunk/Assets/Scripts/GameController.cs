using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum Mode
{
	GAME,
	EDITOR
}


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
	HEAL,
	NONE,
	ANY
}

public enum CellType
{
	SEA,
	REEFS,
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
	public Field f;
	private ClickEventsController clickEventsController;
	public Player currentPlayer;
	private bool gameOver;
	private HUD hud;
	public GAME_STATE gameState;
	public readonly float HIT_PROBABILITY = 1.0f;
	private Mode mode = Mode.GAME;
	public LevelData levelData;



	void Awake()
	{		
		gameState = GAME_STATE.INITIALIZATION;
		MakeSingleton();

		levelData = new LevelData("Levels/level_001");

		currentPlayer = Player.PLAYER_1;
		gameOver = false;

		f = new Field(levelData.FieldWidth, levelData.FieldHeight);

		List<Cell> cells = f.GetAllCells();
		foreach (Cell cell in levelData.Cells)
		{
			cells[levelData.FieldWidth * cell.Y + cell.X].CellType = cell.CellType;
		}

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();

		foreach (Unit u in levelData.Units)
		{
			f.AddUnit(u);
		}

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		Utils.fa = fa;

		clickEventsController = fieldObject.AddComponent<ClickEventsController>();



	}


	void Start()
	{
		hud = GameObject.Find("HUD").AddComponent<HUD>();
		f.SelectRandomUnit(currentPlayer);
		hud.Init(f, currentWeather);
		clickEventsController.Init(fa, f);
		fa.Init(f);
			
		fa.UpdateField();



		CameraDrag cd = Camera.main.gameObject.AddComponent<CameraDrag>();
		cd.Init(f, fa);
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
		HealUnits(currentPlayer);
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


	private void HealUnits( Player player )
	{
		foreach( Unit unit in f.GetAlivePlayerUnits(player)){
			if (unit.Hp < unit.Max_hp)
			{
				if (unit.Unit_class != "fort")
				{
					Cell cell = f.GetCell(unit.Position.x, unit.Position.y);
					Unit fort = cell.BelongsToFort;
					if (fort != null && fort.Player == currentPlayer)
					{
						if (!unit.MovementDone && !unit.FireDone)
						{
							unit.Hp++;
							Debug.Log("Unit healed");
						}
					}
				}
			}
		}
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

	public Mode Mode
	{
		get
		{
			return mode;
		}

		set
		{
			mode = value;
		}
	}

	
}
