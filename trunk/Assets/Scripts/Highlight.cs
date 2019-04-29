using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight {
	private float angle;
	private float angle_rad;
	private float scaleY;
	private float viewAngle;
	private float fieldZeroX;
	private float fieldZeroY;
	private float cellWidth;
	private float cellHeight;
	private Vector2 fieldSize;

	public Highlight(float angle, float scaleY, float fieldZeroX, float fieldZeroY, Vector2 fieldSize, float cellWidth, float cellHeight)
	{
		this.angle = angle;
		this.scaleY = scaleY;
		this.fieldZeroX = fieldZeroX;
		this.fieldZeroY = fieldZeroY;
		this.fieldSize = fieldSize;
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;

		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;

	}

	public void HighlightArea(Vector2 positionOnField, int radius, string type)
	{
		int x = (int)positionOnField.x;
		int y = (int)positionOnField.y;
		Weather currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Mathf.Abs(rel_x) == Mathf.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x <= fieldSize.x && x + rel_x >= 1 && y + rel_y <= fieldSize.y && y + rel_y >= 1)
					{
						if (type == "move")
						{
							if (currentWeather.currentWeatherType == Weather.weather_type.WIND)
							{
								int rad = Mathf.Max(Mathf.Abs(rel_x), Mathf.Abs(rel_y));
								if (rad <= radius - currentWeather.DistanceToCurrentWind(rel_x, rel_y))
								{									
									AddCellAppearance( new Vector2(x + rel_x, y + rel_y));
								}
							}
							else
							{
								if (currentWeather.currentWeatherType == Weather.weather_type.CALM)
								{
									AddCellAppearance(new Vector2(x + rel_x, y + rel_y));
								}
							}
						}
						else
						{
							AddCellAppearance(new Vector2(x + rel_x, y + rel_y));
						}
					}
				}
			}
		}
	}

	private void AddCellAppearance(Vector2 pos)
	{
		GameObject cellGameObject = new GameObject();
		CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();
		ca.Init(angle, viewAngle, cellWidth, cellHeight);
		ca.SetPosition(pos, new Vector2(fieldZeroX, fieldZeroY));
	}
}
