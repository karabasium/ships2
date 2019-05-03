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
	private Vector2Int fieldSize;
	private List<GameObject> cellGameObjects;
	private GameObject cellParent;
	private List<Vector2Int> highlightedCellsIndexes;
	private Color canMoveColor;
	private Color canFireColor;

	public void Init(float angle, float scaleY, float fieldZeroX, float fieldZeroY, Vector2Int fieldSize, float cellWidth, float cellHeight)
	{
		this.angle = angle;
		this.scaleY = scaleY;
		this.fieldZeroX = fieldZeroX;
		this.fieldZeroY = fieldZeroY;
		this.fieldSize = fieldSize;
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		cellGameObjects = new List<GameObject>();
		highlightedCellsIndexes = new List<Vector2Int>();

		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;
		Debug.Log("Highlight constructor: angle = " + this.angle.ToString() + " viewAngle = " + viewAngle.ToString());
		cellParent = new GameObject();
		cellParent.name = "highlightCellsParent";

		canMoveColor = new Color(152f / 255f, 205f / 255f, 250f / 255f);
		canFireColor = new Color(250f / 255f, 136f / 255f, 136f / 255f);
	}

	public void HighlightArea(Vector2Int positionOnField, int radius, string type)
	{
		int x = positionOnField.x;
		int y = positionOnField.y;
		Weather currentWeather = new Weather();
		currentWeather.Init();
		currentWeather.SetWeather();
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
									AddCellAppearance( new Vector2(x + rel_x, y + rel_y), type);
									highlightedCellsIndexes.Add( new Vector2Int(x + rel_x, y + rel_y) );
								}
								
							}
							else
							{
								if (currentWeather.currentWeatherType == Weather.weather_type.CALM)
								{
									AddCellAppearance(new Vector2(x + rel_x, y + rel_y), type);
								}
							}
						}
						else
						{
							AddCellAppearance(new Vector2(x + rel_x, y + rel_y), type); //Cells under fire highlight
						}
					}
				}
			}
		}
	}

	private void AddCellAppearance(Vector2 pos, string type)
	{
		GameObject cellGameObject = new GameObject();
		cellGameObject.name = "cellGameObject";
		CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();

		cellGameObject.transform.parent = cellParent.transform;

		cellGameObjects.Add(cellGameObject);

		ca.Init(angle, viewAngle, cellWidth, cellHeight, cellGameObject, type);
		ca.SetPosition(pos, new Vector2(fieldZeroX, fieldZeroY));

		Color defaultColor = new Color(1f, 1f, 1f);

		if (type == "move")
		{
			ca.SetColor(canMoveColor);
		}
		else if (type == "fire")
		{
			ca.SetColor( canFireColor );
		}
		else
		{
			ca.SetColor( defaultColor );
		}
	}

	public void ResetHighlight()
	{
		foreach(GameObject go in cellGameObjects)
		{
			Destroy(go);
		}
		highlightedCellsIndexes = new List<Vector2Int>();
	}

	public List<Vector2Int> GetHighlightedCellsIndexes()
	{
		return highlightedCellsIndexes;
	}
}
