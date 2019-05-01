using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
	private float angle;
	private float angle_rad;
	private float scaleY;
	private float viewAngle;
	private float fieldZeroX;
	private float fieldZeroY;
	private float cellWidth;
	private float cellHeight;
	private Vector2 fieldSize;
	private List<GameObject> cellGameObjects;

	/*public Highlight(float angle, float scaleY, float fieldZeroX, float fieldZeroY, Vector2 fieldSize, float cellWidth, float cellHeight)
	{
		this.angle = angle;
		this.scaleY = scaleY;
		this.fieldZeroX = fieldZeroX;
		this.fieldZeroY = fieldZeroY;
		this.fieldSize = fieldSize;
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		cellGameObjects = new List<GameObject>();

		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;
		Debug.Log("Highlight constructor: angle = " + this.angle.ToString() + " viewAngle = " + viewAngle.ToString());
	}*/

	public void Init(float angle, float scaleY, float fieldZeroX, float fieldZeroY, Vector2 fieldSize, float cellWidth, float cellHeight)
	{
		this.angle = angle;
		this.scaleY = scaleY;
		this.fieldZeroX = fieldZeroX;
		this.fieldZeroY = fieldZeroY;
		this.fieldSize = fieldSize;
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		cellGameObjects = new List<GameObject>();

		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;
		Debug.Log("Highlight constructor: angle = " + this.angle.ToString() + " viewAngle = " + viewAngle.ToString());
	}

	public void HighlightArea(Vector2Int positionOnField, int radius, string type)
	{
		int x = positionOnField.x;
		int y = positionOnField.y;
		Weather currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
		////Debug.Log("HighlightArea: radius = " + radius.ToString() + ". type = " + type + " Position = " + positionOnField.ToString());
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
								//////Debug.Log("HighlightArea: rad = " + rad.ToString());
								if (rad <= radius - currentWeather.DistanceToCurrentWind(rel_x, rel_y))
								{									
									AddCellAppearance( new Vector2(x + rel_x, y + rel_y));
									//////Debug.Log("cell added");
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
		cellGameObjects.Add(cellGameObject);
		ca.Init(angle, viewAngle, cellWidth, cellHeight);
		ca.SetPosition(pos, new Vector2(fieldZeroX, fieldZeroY));
	}

	public void ResetHighlight()
	{
		foreach(GameObject go in cellGameObjects)
		{
			Destroy(go); // not working yet
		}
	}
}
