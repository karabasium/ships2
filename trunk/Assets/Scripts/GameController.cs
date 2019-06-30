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
	FIELD_APPEARANCE_NEED_TO_UPDATE,
	GAME_OVER
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
	private HUD hud;
	public GAME_STATE gameState;
	public readonly float HIT_PROBABILITY = 1.0f;
	private Mode mode = Mode.GAME;
	public LevelData levelData;



	void Awake()
	{
		MakeSingleton();
		Init();
	}

	public void Init()
	{
		gameState = GAME_STATE.INITIALIZATION;
		currentPlayer = Player.PLAYER_1;

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();

		levelData = new LevelData("Levels/level_001");
		f = new Field(levelData.FieldWidth, levelData.FieldHeight);
		List<Cell> cells = f.GetAllCells();
		foreach (Cell cell in levelData.Cells)
		{
			cells[levelData.FieldWidth * cell.Y + cell.X].CellType = cell.CellType;
		}

		foreach (Unit u in levelData.Units)
		{
			f.AddUnit(u);
		}
		Utils.field = f;

		if (fieldObject == null)
		{
			fieldObject = new GameObject();
		}
		if (fieldObject.GetComponent<FieldAppearance>() != null)
		{
			Destroy(fieldObject.GetComponent<FieldAppearance>());
		}
		fa = fieldObject.AddComponent<FieldAppearance>();
		Utils.fa = fa;

		if (fieldObject.GetComponent<ClickEventsController>() != null)
		{
			Destroy(fieldObject.GetComponent<ClickEventsController>());
		}
		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		if (GameObject.Find("HUD").GetComponent<HUD>() != null)
		{
			Destroy(GameObject.Find("HUD").GetComponent<HUD>());
		}
		hud = GameObject.Find("HUD").AddComponent<HUD>();

		//f.SelectRandomUnit(currentPlayer);
		hud.Init(f, currentWeather);
		clickEventsController.Init(fa, f);
		fa.Init(f);

		fa.UpdateField();


		if (Camera.main.gameObject.AddComponent<CameraDrag>() != null)
		{
			Destroy(Camera.main.gameObject.AddComponent<CameraDrag>());
		}
		CameraDrag cd = Camera.main.gameObject.AddComponent<CameraDrag>();
		cd.Init(f, fa);
	}


	void Start()
	{

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
		if (gameState != GAME_STATE.GAME_OVER)
		{
			Player winner = WhoIsWinner();
			if (winner == Player.NONE)
			{
				if (PlayerDidAnyPossibleActions())
				{
					SetNextPlayerAsActive();
				}
			}
			else
			{
				gameState = GAME_STATE.GAME_OVER;
				hud.ShowVictoryScreen(winner);
				fa.Clear();
				Debug.Log("GAME OVER! Winner is player " + WhoIsWinner().ToString());
			}

			if (currentWeather.needPerformStormActions)
			{
				f.ReleaseUnitsSelection();
				f.StormMoveAllShips();
				currentWeather.needPerformStormActions = false;
			}
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
		//f.SelectRandomUnit(currentPlayer);
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
