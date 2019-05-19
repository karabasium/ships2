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
	}


	void Update () {
		if (field.needHUDupdate)
		{
			UpdateUIShipInfo(field.GetLastSelectedUnit());
			field.needHUDupdate = false;
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

	public void UpdateUIShipInfo( Unit u)
	{
		hpLabel.text = "HP";		
		hpValue.text = u.hp.ToString();
		shotsCountLabel.text = "Shots count";
		shotsCountValue.text = u.shots.ToString();

	}


	public void ResetUIShipInfo()
	{
		hpValue.text = "";
		shotsCountValue.text = "";
	}
}
