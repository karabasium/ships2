using System.Collections;
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

		/*List<Vector2Int> directions = new List<Vector2Int>() { new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),
														 new Vector2Int(-1, 1),  new Vector2Int(0, 0),   new Vector2Int(1, 0),
														 new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1)};*/

		List<Vector2Int> forbiddenDirectionIndexes = new List<Vector2Int>();

		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				Vector2Int direction = new Vector2Int(rel_x, rel_y);
				direction = Normalize(direction);
				if (forbiddenDirectionIndexes.Contains(direction))
				{
					continue;
				}

				if (Mathf.Abs(rel_x) == Mathf.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x < fieldSize.x && x + rel_x >= 0 && y + rel_y < fieldSize.y && y + rel_y >= 0)
					{
						Cell c = allFieldCells[fieldSize.x * (y + rel_y) + (x + rel_x)];
						if (c.CellType == CellType.LAND)
						{
							forbiddenDirectionIndexes.Add(direction);
							continue;
						}
						if (type == Action.MOVE)
						{
							if (currentWeather.currentWeatherType == Weather_type.WIND)
							{
								int rad = Mathf.Max(Mathf.Abs(rel_x), Mathf.Abs(rel_y));
								if (rad <= radius - currentWeather.DistanceToCurrentWind(rel_x, rel_y))
								{									
									cells.Add(c);
								}

							}
							else if (currentWeather.currentWeatherType == Weather_type.CALM)
							{
								cells.Add(c);
							}
							else if (currentWeather.currentWeatherType == Weather_type.STORM)
							{
								if (currentWeather.DistanceToCurrentWind(rel_x, rel_y) == 0)
								{
									cells.Add(c);
								}
							}
						}
						else
						{
							cells.Add(c); //Cells under fire highlight
						}
					}
				}
			}
		}

		return cells;
	}

	private Vector2Int Normalize( Vector2Int v)
	{
		int x = v.x;
		int y = v.y;
		if (x != 0)
		{
			x = x / Mathf.Abs(x);
		}
		if (y != 0)
		{
			y = y / Mathf.Abs(y);
		}
		return new Vector2Int(x, y);
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
