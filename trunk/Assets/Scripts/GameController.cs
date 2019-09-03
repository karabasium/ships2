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
	CALM,
	UNDEFINED
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
	public FieldAppearance fa;
	public Field f;
	private ClickEventsController clickEventsController;
	public Player currentPlayer;
	public HUD hud;
	public GAME_STATE gameState;
	public readonly float HIT_PROBABILITY = 1.0f;
	private Mode mode = Mode.GAME;
	public LevelData levelData;
	public CameraDrag cd;


	void Awake()
	{
		MakeSingleton();
		Init();
	}

	public void Init()
	{
		ChangeState(GAME_STATE.INITIALIZATION);
		currentPlayer = Player.PLAYER_1;		

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
		hud.Init(f);
		clickEventsController.Init(fa, f);
		fa.Init(f);

		fa.UpdateField();


		if (Camera.main.gameObject.GetComponent<CameraDrag>() != null)
		{
			Destroy(Camera.main.gameObject.GetComponent<CameraDrag>());
		}
		cd = Camera.main.gameObject.AddComponent<CameraDrag>();
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
				ChangeState(GAME_STATE.GAME_OVER);
				hud.ShowVictoryScreen(winner);
				fa.Clear();
				Debug.Log("GAME OVER! Winner is player " + WhoIsWinner().ToString());
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
		f.ReleaseUnitsSelection();		
		f.RefreshPlayerUnits(currentPlayer);		
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

	public void ChangeState( GAME_STATE new_game_state)
	{
		gameState = new_game_state;
		Debug.Log("Game state: " + gameState.ToString());
	}
}
