using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	private Text hpValue;
	private Text hpLabel;
	private Text shotsCountValue;
	private Text shotsCountLabel;
	public bool needUpdate;
	private Field field;
	private Weather weather;
	private WeatherAppearance wa;
	private Button nextTurnButton;
	private Button switchModeButton;
	private ToggleGroup selectObjectInEditorToggleGroup;
	private ToggleGroup playerSelectorInEditorToggleGroup;
	private CanvasGroup editorCanvas;
	private CanvasGroup victoryScreenCanvas;
	private Button playAgain;
	private Text whoIsWinnerText;

	// Use this for initialization
	void Start () {
	}

	public void Init(Field field, Weather weather)
	{
		this.field = field;
		this.weather = weather;
		hpValue = GameObject.Find("ShipHPValue").GetComponent<Text>();
		hpLabel = GameObject.Find("ShipInfoHPLabel").GetComponent<Text>();
		shotsCountValue = GameObject.Find("ShipShotsCountValue").GetComponent<Text>();
		shotsCountLabel = GameObject.Find("ShipInfoShotsLabel").GetComponent<Text>();
		wa = new WeatherAppearance( weather );
		wa.UpdateWeatherAppearance();

		nextTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
		nextTurnButton.onClick.AddListener(() => NextTurn());

		switchModeButton = GameObject.Find("SwitchMode").GetComponent<Button>();
		switchModeButton.GetComponentInChildren<Text>().text = "Switch to editor";
		switchModeButton.onClick.AddListener(() => SwitchMode());

		selectObjectInEditorToggleGroup = GameObject.Find("ObjectSelector").GetComponent<ToggleGroup>();
		playerSelectorInEditorToggleGroup = GameObject.Find("PlayerSelector").GetComponent<ToggleGroup>();

		editorCanvas = GameObject.Find("EditorCanvas").GetComponent<CanvasGroup>();
		editorCanvas.blocksRaycasts = false;
		editorCanvas.alpha = 0.0f;

		victoryScreenCanvas = GameObject.Find("VictoryScreenCanvas").GetComponent<CanvasGroup>();
		victoryScreenCanvas.blocksRaycasts = false;
		victoryScreenCanvas.alpha = 0.0f;

		playAgain = GameObject.Find("PlayAgainButton").GetComponent<Button>();
		playAgain.onClick.AddListener(() => PlayAgain());

		whoIsWinnerText = GameObject.Find("WhoIsWinnerText").GetComponent<Text>();
	}


	void Update () {
		if (field.NeedHUDupdate)
		{
			UpdateUIShipInfo(field.GetLastSelectedUnit());
			field.NeedHUDupdate = false;
		}
		if (weather.needHUDUpdate)
		{
			wa.UpdateWeatherAppearance();
			weather.needHUDUpdate = false;
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

	public void UpdateUIShipInfo( Unit u)
	{
		if (u == null)
		{
			ResetUIShipInfo();
			return;
		}
		hpLabel.text = "HP";		
		hpValue.text = u.Hp.ToString();
		shotsCountLabel.text = "Shots count";
		shotsCountValue.text = u.Shots.ToString();

	}


	public void ResetUIShipInfo()
	{
		hpLabel.text = "";
		shotsCountLabel.text = "";
		hpValue.text = "";
		shotsCountValue.text = "";
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
}
