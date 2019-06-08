using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

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
		cells[16].CellType = CellType.LAND;
		cells[28].CellType = CellType.LAND;
		cells[40].CellType = CellType.LAND;

		cells[50].CellType = CellType.REEFS;


		Unit fort = new Unit("fort", Player.PLAYER_1);
		Unit u3 = new Unit("brig", Player.PLAYER_1 );
		Unit u1 = new Unit("brig", Player.PLAYER_1);
		Unit u2 = new Unit("brig", Player.PLAYER_1);


		Unit u4 = new Unit("brig", Player.PLAYER_2);		

		f.AddUnit(new Vector2Int(3, 3), u1);
		f.AddUnit(new Vector2Int(3, 1), fort);
		f.AddUnit(new Vector2Int(3, 3), u2);
		f.AddUnit(new Vector2Int(2, 2), u3);

		f.AddUnit(new Vector2Int(3, 7), u4);

		fieldObject = new GameObject();
		fa = fieldObject.AddComponent<FieldAppearance>();
		Utils.fa = fa;

		clickEventsController = fieldObject.AddComponent<ClickEventsController>();

		currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.RefreshWeather();


		Dictionary<string, List<Dictionary<string, string>>> levelData = new Dictionary<string, List<Dictionary<string, string>>>();
		TextAsset textAsset = (TextAsset)Resources.Load("Levels/level_001");

		XmlDocument xmlDoc = new XmlDocument();

		xmlDoc.LoadXml(textAsset.text);
		XmlNodeList cellNodes = xmlDoc.GetElementsByTagName("cell");
		levelData.Add("cells", new List<Dictionary<string, string>>());
		foreach (XmlNode node in cellNodes)
		{
			Debug.Log("type = " + node.Attributes["type"].Value);
			Dictionary<string, string> cellData = new Dictionary<string, string>();
			cellData.Add("type", node.Attributes["type"].Value);
			cellData.Add("x", node.Attributes["x"].Value);
			cellData.Add("y", node.Attributes["y"].Value);
			levelData["cells"].Add(cellData);
		}

		XmlNodeList unitNodes = xmlDoc.GetElementsByTagName("unit");
		levelData.Add("units", new List<Dictionary<string, string>>());
		foreach (XmlNode node in unitNodes)
		{
			Debug.Log("class = " + node.Attributes["class"].Value);
			Dictionary<string, string> unitData = new Dictionary<string, string>();
			unitData.Add("class", node.Attributes["class"].Value);
			unitData.Add("x", node.Attributes["x"].Value);
			unitData.Add("y", node.Attributes["y"].Value);
			levelData["units"].Add(unitData);
		}

		levelData.Add("level", new List<Dictionary<string, string>>());
		XmlNodeList levelNodes = xmlDoc.GetElementsByTagName("level");
		Dictionary<string, string> levelParameters = new Dictionary<string, string>();
		levelParameters.Add("width", levelNodes[0].Attributes["width"].Value);
		levelParameters.Add("height", levelNodes[0].Attributes["height"].Value);

		levelData["level"].Add(levelParameters);
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
}
