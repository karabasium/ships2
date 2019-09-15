using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	private Text hpValue;
	private Text hpLabel;
	private GameObject shotToggle1;
	private GameObject shotToggle2;
	private GameObject shotToggle3;
	private List<Toggle> shotsToggle;
	private Text shotsCountLabel;
	public bool needUpdate;
	private Field field;
	private Button nextTurnButton;
	private Button switchModeButton;
	private ToggleGroup selectObjectInEditorToggleGroup;
	private ToggleGroup playerSelectorInEditorToggleGroup;
	private CanvasGroup editorCanvas;
	private CanvasGroup victoryScreenCanvas;
	private Button playAgain;
	private Text whoIsWinnerText;
	private Text weatherName;

	// Use this for initialization
	void Start () {		
	}

	public void Init(Field field)
	{
		this.field = field;
		hpValue = GameObject.Find("ShipHPValue").GetComponent<Text>();
		hpLabel = GameObject.Find("ShipInfoHPLabel").GetComponent<Text>();
		shotsCountLabel = GameObject.Find("ShipInfoShotsLabel").GetComponent<Text>();

		nextTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
		nextTurnButton.onClick.AddListener(() => NextTurn());

		switchModeButton = GameObject.Find("SwitchMode").GetComponent<Button>();
		switchModeButton.GetComponentInChildren<Text>().text = "Switch to editor";
		switchModeButton.onClick.AddListener(() => SwitchMode());

		selectObjectInEditorToggleGroup = GameObject.Find("ObjectSelector").GetComponent<ToggleGroup>();
		playerSelectorInEditorToggleGroup = GameObject.Find("PlayerSelector").GetComponent<ToggleGroup>();

		shotToggle1 = GameObject.Find("shot1");
		shotToggle2 = GameObject.Find("shot2");
		shotToggle3 = GameObject.Find("shot3");

		shotsToggle = new List<Toggle>();
		shotsToggle.Add(shotToggle1.GetComponent<Toggle>());
		shotsToggle.Add(shotToggle2.GetComponent<Toggle>());
		shotsToggle.Add(shotToggle3.GetComponent<Toggle>());

		editorCanvas = GameObject.Find("EditorCanvas").GetComponent<CanvasGroup>();
		editorCanvas.blocksRaycasts = false;
		editorCanvas.alpha = 0.0f;

		victoryScreenCanvas = GameObject.Find("VictoryScreenCanvas").GetComponent<CanvasGroup>();
		victoryScreenCanvas.blocksRaycasts = false;
		victoryScreenCanvas.alpha = 0.0f;

		playAgain = GameObject.Find("PlayAgainButton").GetComponent<Button>();
		playAgain.onClick.AddListener(() => PlayAgain());

		whoIsWinnerText = GameObject.Find("WhoIsWinnerText").GetComponent<Text>();

		weatherName = GameObject.Find("WeatherValue").GetComponent<Text>();

		ResetUIShipInfo();
	}


	void Update () {
		if (field.NeedHUDupdate)
		{
			Unit u = field.GetLastSelectedUnit();
			UpdateUIShipInfo(u);
			field.NeedHUDupdate = false;
		}
	}

	public void NextTurn()
	{
		GameController.instance.SetNextPlayerAsActive();
	}

	public void SwitchMode()
	{
		SwitchEditor();
		GameController.instance.levelData.Save();
		if (GameController.instance.Mode == Mode.GAME)
		{
			GameController.instance.Mode = Mode.EDITOR;
			switchModeButton.GetComponentInChildren<Text>().text = "Switch to Game";
			
		}
		else
		{
			GameController.instance.Mode = Mode.GAME;
			switchModeButton.GetComponentInChildren<Text>().text = "Switch to Editor";
		}
	}

	public void ShowUnitAttachedButton(Unit u)
	{
		if(u.MovementDone || u.weather.currentWeatherType != Weather_type.UNDEFINED)
		{
			return;
		}

		GameObject unitGameObject = Utils.GetUnitGameObject(u);
		Canvas c = unitGameObject.GetComponentInChildren<Canvas>();
		if (c != null)
		{
			c.enabled = true;
			Debug.Log("Canvas enabled");
		}
		else
		{
			Button b = AttachButtonToUnit( unitGameObject );
			b.onClick.AddListener(() => ChangeWeather(u));
			Debug.Log("Canvas created");
		}
	}

	private Button AttachButtonToUnit(GameObject unitGameObject)
	{
		GameObject canvasObject = new GameObject() { name = "attachedButtonCanvas" };
		Canvas c = canvasObject.AddComponent<Canvas>();
		c.renderMode = RenderMode.WorldSpace;
		GraphicRaycaster gr = canvasObject.AddComponent<GraphicRaycaster>();

		RectTransform rt = c.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(2, 2);


		Collider2D collider = unitGameObject.GetComponent<Collider2D>();
		float height = collider.bounds.size.y;

		c.transform.parent = unitGameObject.transform;
		c.transform.position = unitGameObject.transform.position + new Vector3(0, height, 0);
		c.sortingLayerName = "HUD";

		GameObject button = (GameObject)Instantiate(Resources.Load("Prefabs/AttachedButtonChangeWeather"));
		button.transform.parent = c.transform;
		button.transform.position = c.transform.position;

		Button b = button.GetComponent<Button>();
		return b;		
	}

	public void HideUnitAttachedButton(Unit u)
	{
		GameObject unitGameObject = Utils.GetUnitGameObject(u);
		Canvas c = unitGameObject.GetComponentInChildren<Canvas>();
		if (c != null)
		{
			c.enabled = false;
			Debug.Log("Canvas disabled");
		}				
	}

	public void HideAllUnitsAttachedButtons()
	{
		foreach(Unit u in field.GetAlivePlayerUnits(GameController.instance.currentPlayer))
		{
			HideUnitAttachedButton(u);
		}
	}

	private void ChangeWeather(Unit u1)
	{
		Debug.Log("Change weather");
		Weather w = new Weather();
		w.RefreshWeather();
		Unit lastSelected = field.GetLastSelectedUnit();
		foreach (Unit u in field.GetSelectedUnits())
		{
			if (u.weather.currentWeatherType == Weather_type.UNDEFINED && !u.MovementDone)
			{
				Weather_type wt = u.weather.RefreshWeather(w);
				if (wt == Weather_type.STORM)
				{
					field.StormDriftsUnit(u);
				}
				HideUnitAttachedButton(u);
			}
		}
		field.ReleaseUnitsSelection();
		field.AddUnitToSelectedUnits(lastSelected);
		UpdateWeatherAppearance(w);
		GameController.instance.ChangeState(GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE);
	}

	public void UpdateUIShipInfo( Unit u)
	{
		if (u == null)
		{
			ResetUIShipInfo();
			return;
		}

		UpdateWeatherAppearance(u.weather);

		hpLabel.text = "HP";		
		hpValue.text = u.Hp.ToString();
		shotsCountLabel.text = "Shots count";

		if (u.Shots == 0)
		{
			shotToggle1.gameObject.SetActive(false);
			shotToggle2.gameObject.SetActive(false);
			shotToggle3.gameObject.SetActive(false);
		}
		else if (u.Shots == 1)
		{
			shotToggle1.gameObject.SetActive(true);
			shotToggle2.gameObject.SetActive(false);
			shotToggle3.gameObject.SetActive(false);

			shotToggle1.GetComponent<Toggle>().isOn = true;
			shotToggle1.GetComponent<Toggle>().enabled = false;
			shotToggle2.GetComponent<Toggle>().isOn = false;
			shotToggle3.GetComponent<Toggle>().isOn = false;


		}
		else if (u.Shots == 2)
		{
			shotToggle1.gameObject.SetActive(true);
			shotToggle2.gameObject.SetActive(true);
			shotToggle3.gameObject.SetActive(false);

			shotToggle1.GetComponent<Toggle>().isOn = true;
			shotToggle1.GetComponent<Toggle>().enabled = false;
			shotToggle2.GetComponent<Toggle>().isOn = false;
			shotToggle3.GetComponent<Toggle>().isOn = false;
		}
		else if (u.Shots == 3)
		{
			shotToggle1.gameObject.SetActive(true);
			shotToggle2.gameObject.SetActive(true);
			shotToggle3.gameObject.SetActive(true);

			shotToggle1.GetComponent<Toggle>().isOn = true;
			shotToggle1.GetComponent<Toggle>().enabled = false;
			shotToggle2.GetComponent<Toggle>().isOn = false;
			shotToggle3.GetComponent<Toggle>().isOn = false;
		}
	}

	public int GetShotsCountUserSelected()
	{
		int shots = 0;
		foreach(Toggle t in shotsToggle)
		{
			if (t.isOn)
			{
				shots++;
			}
		}
		return shots;
	}


	public void ResetUIShipInfo()
	{
		hpLabel.text = "";
		shotsCountLabel.text = "";
		hpValue.text = "";
		shotToggle1.gameObject.SetActive(false);
		shotToggle2.gameObject.SetActive(false);
		shotToggle3.gameObject.SetActive(false);
	}
	
	public string GetObjectTypeToPlace()
	{
		Toggle activeToggle = selectObjectInEditorToggleGroup.ActiveToggles().FirstOrDefault();
		return activeToggle.gameObject.name;
	}

	public Player GetPlayer()
	{
		Toggle playerToggle = playerSelectorInEditorToggleGroup.ActiveToggles().FirstOrDefault();
		string toggleName = playerToggle.gameObject.name;
		if (toggleName == "Player1")
		{
			return Player.PLAYER_1;
		}
		else if (toggleName == "Player2")
		{
			return Player.PLAYER_2;
		}
		else
		{
			return Player.NONE;
		}
	}

	public void SwitchEditor()
	{
		if (editorCanvas.blocksRaycasts == false)
		{
			editorCanvas.blocksRaycasts = true;
			editorCanvas.alpha = 1.0f;
		}
		else
		{
			editorCanvas.blocksRaycasts = false;
			editorCanvas.alpha = 0.0f;
		}
	}

	public void ShowVictoryScreen( Player winner)
	{
		victoryScreenCanvas.blocksRaycasts = true;
		victoryScreenCanvas.alpha = 1.0f;
		string winnerPlayer = "Winner is Player 1!";
		if (winner == Player.PLAYER_2)
		{
			winnerPlayer = "Winner is Player 2!";
		}
		whoIsWinnerText.text = winnerPlayer;
	}

	public void PlayAgain()
	{
		GameController.instance.Init();
		victoryScreenCanvas.blocksRaycasts = false;
		victoryScreenCanvas.alpha = 0.0f;
	}

	public void UpdateWeatherAppearance( Weather weather)
	{
		Debug.Log("weather.curWindIndex = " + weather.curWindIndex.ToString());

		if (weather.currentWeatherType == Weather_type.WIND)
		{

			if (weather.curWindIndex == 0)
			{
				//arrow.transform.Rotate(Vector3.back * 135);
				weatherName.text = "NW Breeze";
			}
			else if (weather.curWindIndex == 1)
			{
				//arrow.transform.Rotate(Vector3.back * 90);
				weatherName.text = "N Breeze";
			}
			else if (weather.curWindIndex == 2)
			{
				//arrow.transform.Rotate(Vector3.back * 45);
				weatherName.text = "NE Breeze";
			}
			else if (weather.curWindIndex == 3)
			{
				//arrow.transform.Rotate(Vector3.back * 0);
				weatherName.text = "E Breeze";
			}
			else if (weather.curWindIndex == 4)
			{
				//arrow.transform.Rotate(Vector3.forward * 45);
				weatherName.text = "SE Breeze";
			}
			else if (weather.curWindIndex == 5)
			{
				//arrow.transform.Rotate(Vector3.forward * 90);
				weatherName.text = "S Breeze";
			}
			else if (weather.curWindIndex == 6)
			{
				//arrow.transform.Rotate(Vector3.forward * 135);
				weatherName.text = "SW Breeze";
			}
			else if (weather.curWindIndex == 7)
			{
				//arrow.transform.Rotate(Vector3.back * -180);
				weatherName.text = "W Breeze";
			}
		}
		else if (weather.currentWeatherType == Weather_type.STORM)
		{
			weatherName.text = "Storm";
		}
		else if (weather.currentWeatherType == Weather_type.CALM)
		{
			weatherName.text = "Calm";
		}
		else
		{
			weatherName.text = "";
		}
	}
}
