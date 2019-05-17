using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAppearance {
	private Weather weather;

	private Text weatherName;

	public WeatherAppearance(Weather w)
	{
		weather = w;
		weatherName = GameObject.Find("WeatherValue").GetComponent<Text>();
	}

	public void UpdateWeatherAppearance()
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
			weatherName.text = "Armageddon";
		}		
	}

}
