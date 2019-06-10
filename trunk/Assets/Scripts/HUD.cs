using System.Collections;
using System.Collections.Generic;
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
}
