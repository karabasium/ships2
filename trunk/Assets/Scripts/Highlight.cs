using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight {
	private Vector2Int fieldSize;
	public List<Cell> canFireCells;
	public List<Cell> canMoveCells;
	private List<Cell> allFieldCells;

	public Highlight(int fieldWidth, int fieldHeight, List<Cell> cells)
	{
		
		fieldSize = new Vector2Int( fieldWidth, fieldHeight);

		canFireCells = new List<Cell>();
		canMoveCells = new List<Cell>();
		allFieldCells = cells;
	}

	public List<Cell> GetHighlightedCells(Vector2Int positionOnField, int radius, string type)
	{
		int x = positionOnField.x;
		int y = positionOnField.y;
		Weather currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();

		List<Cell> cells = new List<Cell>();

		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Mathf.Abs(rel_x) == Mathf.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x < fieldSize.x && x + rel_x >= 0 && y + rel_y < fieldSize.y && y + rel_y >= 0)
					{
						if (type == "move")
						{
							if (currentWeather.currentWeatherType == Weather.weather_type.WIND)
							{
								int rad = Mathf.Max(Mathf.Abs(rel_x), Mathf.Abs(rel_y));
								if (rad <= radius - currentWeather.DistanceToCurrentWind(rel_x, rel_y))
								{
									cells.Add(allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)]);
								}

							}
							else
							{
								if (currentWeather.currentWeatherType == Weather.weather_type.CALM)
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
		
		if (!u.movementDone)
		{
			canMoveCells = GetHighlightedCells(u.GetPosition(), u.move_range, "move");
		}
		if (!u.fireDone)
		{
			canFireCells = GetHighlightedCells(u.GetPosition(), u.fire_range, "fire");
		}
	}

	public void ResetHighlightedCellsLists( string type="all" )
	{
		if (type == "move")
		{
			canMoveCells = new List<Cell>();
		}
		else if (type == "fire")
		{
			canFireCells = new List<Cell>();
		}
		else if (type == "all")
		{
			canFireCells = new List<Cell>();
			canMoveCells = new List<Cell>();
		}
		else
		{
			Debug.Log("ERROR! ResetHighlightedCellsPositionsLists(): Unknown reset highlight type = " + type.ToString());
		}
	}
}
