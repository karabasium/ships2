﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weather
{
	private int[][] directions = new int[][] { new int[] { -1, 1  }, new int[] { 0, 1 },   new int[] { 1, 1  },
											new int[] { -1, 0  },                       new int[] { 1, 0  },
											new int[] { -1, -1 }, new int[] { 0, -1 }, new int[]  { 1,-1 }};

	private List<int[]> dirs = new List<int[]>();

	private int[] curWind;
	public int curWindIndex;

	public Weather_type currentWeatherType;
	public bool needHUDUpdate;
	public bool needPerformStormActions;

	public Weather()
	{
		dirs.Add(new int[] { -1, 1 });
		dirs.Add(new int[] { 0, 1 });
		dirs.Add(new int[] { 1, 1 });
		dirs.Add(new int[] { 1, 0 });
		dirs.Add(new int[] { 1, -1 });
		dirs.Add(new int[] { 0, -1 });
		dirs.Add(new int[] { -1, -1 });
		dirs.Add(new int[] { -1, 0 });

		needHUDUpdate = false;
		needPerformStormActions = false;
		currentWeatherType = Weather_type.UNDEFINED;
	}

	public int DistanceToCurrentWind(int dirX, int dirY)
	{
		int dirIndex = -100;
		for (int i = 0; i < dirs.Count; i++)
		{
			if (dirs[i][0] == System.Math.Sign(dirX) && dirs[i][1] == System.Math.Sign(dirY))
			{
				dirIndex = i;
				break;
			}
		}
		int len1 = System.Math.Abs(curWindIndex - dirIndex);
		int len2 = System.Math.Abs((dirs.Count) - len1);

		return System.Math.Min(len1, len2);
	}

	public Weather_type RefreshWeather( Weather w = null)
	{
		if (w == null)
		{
			currentWeatherType = (Weather_type)Random.Range(0, System.Enum.GetValues(typeof(Weather_type)).Length - 1);
		}
		else
		{
			currentWeatherType = w.currentWeatherType;
		}

		//currentWeatherType = Weather_type.CALM; // FOR DEBUG ONLY!!!

		Debug.Log("WEATHER: " + currentWeatherType.ToString());
		if (currentWeatherType == Weather_type.WIND || currentWeatherType == Weather_type.STORM)
		{
			if (w == null)
			{
				curWindIndex = Random.Range(0, dirs.Count - 1);
			}
			else
			{
				curWindIndex = w.curWindIndex;
			}
			//curWindIndex = 0;  // FOR DEBUG ONLY!!!
			curWind = dirs[curWindIndex];

			if (currentWeatherType == Weather_type.STORM)
			{
				needPerformStormActions = true;
			}
		}
		else
		{
			if (currentWeatherType == Weather_type.CALM)
			{
				curWindIndex = -1;
				curWind = null;
			}
		}
		needHUDUpdate = true;
		return currentWeatherType;
	}
}
