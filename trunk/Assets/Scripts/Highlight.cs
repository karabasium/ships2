﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight {
	private Vector2Int fieldSize;
	private List<Cell> canFireCells;
	private List<Cell> canMoveCells;
	private List<Cell> allFieldCells;

	public List<Cell> CanFireCells
	{
		get
		{
			return canFireCells;
		}

		set
		{
			canFireCells = value;
		}
	}

	public List<Cell> CanMoveCells
	{
		get
		{
			return canMoveCells;
		}

		set
		{
			canMoveCells = value;
		}
	}

	public Highlight(int fieldWidth, int fieldHeight, List<Cell> cells)
	{
		
		fieldSize = new Vector2Int( fieldWidth, fieldHeight);

		CanFireCells = new List<Cell>();
		CanMoveCells = new List<Cell>();
		allFieldCells = cells;
	}

	public List<Cell> GetHighlightedCells(Vector2Int positionOnField, Unit u, Action type)
	{
		int x = positionOnField.x;
		int y = positionOnField.y;
		Weather currentWeather = GameController.instance.currentWeather;
		int radius = 0;

		if (type == Action.MOVE)
		{
			if (currentWeather.currentWeatherType == Weather_type.WIND)
			{
				radius = u.Move_range;
			}
			else if (currentWeather.currentWeatherType == Weather_type.CALM)
			{
				radius = u.Calm_move_range;
			}
			else if (currentWeather.currentWeatherType == Weather_type.STORM)
			{
				radius = u.Storm_drift_range;
			}
		}
		else if (type == Action.FIRE)
		{
			radius = u.Fire_range;
		}

		List<Cell> cells = new List<Cell>();

		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Mathf.Abs(rel_x) == Mathf.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x < fieldSize.x && x + rel_x >= 0 && y + rel_y < fieldSize.y && y + rel_y >= 0)
					{
						if (type == Action.MOVE)
						{
							if (currentWeather.currentWeatherType == Weather_type.WIND)
							{
								int rad = Mathf.Max(Mathf.Abs(rel_x), Mathf.Abs(rel_y));
								if (rad <= radius - currentWeather.DistanceToCurrentWind(rel_x, rel_y))
								{
									cells.Add(allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)]);
								}

							}
							else if (currentWeather.currentWeatherType == Weather_type.CALM)
							{
								cells.Add(allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)]);
							}
							else if (currentWeather.currentWeatherType == Weather_type.STORM)
							{
								if (currentWeather.DistanceToCurrentWind(rel_x, rel_y) == 0)
								{
									cells.Add(allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)]);
								}
							}
						}
						else
						{
							cells.Add(allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)]); //Cells under fire highlight
						}
					}
				}
			}
		}

		return cells;
	}

	public void CreateHighlightedCellsLists(Unit u)
	{
		
		if (!u.MovementDone && GameController.instance.currentWeather.currentWeatherType != Weather_type.STORM)
		{
			CanMoveCells = GetHighlightedCells(u.GetPosition(), u, Action.MOVE);
		}
		if (!u.FireDone)
		{
			CanFireCells = GetHighlightedCells(u.GetPosition(), u, Action.FIRE);
		}
	}

	public void ResetHighlightedCellsLists( Action type=Action.ANY )
	{
		if (type == Action.MOVE)
		{
			CanMoveCells = new List<Cell>();
		}
		else if (type == Action.FIRE)
		{
			CanFireCells = new List<Cell>();
		}
		else if (type == Action.ANY)
		{
			CanFireCells = new List<Cell>();
			CanMoveCells = new List<Cell>();
		}
		else
		{
			Debug.Log("ERROR! ResetHighlightedCellsPositionsLists(): Unknown reset highlight type = " + type.ToString());
		}
	}
}
