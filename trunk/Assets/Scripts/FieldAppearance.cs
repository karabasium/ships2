using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAppearance {
	private Field f;
	private float cellWidth = 1.0f;
	private float cellHeight = 1.0f;
	private float angle = 45.0f;
	private float angle_rad;
	private float scaleY = 0.5f;
	private float fieldZeroX = -2;
	private float fieldZeroY = -2;


	public FieldAppearance( Field f)
	{
		this.f = f;
		angle_rad = Mathf.PI * (angle / 180);

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();
	}

	private void DrawShips()
	{
		
		foreach (Cell cell in f.GetCells())
		{
			if (cell.units.Count == 0) continue;

			float verticalOffset = 3 * cellHeight / 3;
			if (cell.units.Count == 1)
			{
				verticalOffset = 3 * cellHeight / 4;
			}
			else if (cell.units.Count == 2)
			{
				verticalOffset = 9 * cellHeight / 10;
				Debug.Log(verticalOffset);
			}
			for (int i=0; i<cell.units.Count; i++) {
				Vector2 pos = new Vector2(fieldZeroX + cell.x * cellWidth + 3*cellWidth / 4, fieldZeroY + cell.y * cellHeight + verticalOffset - (1*verticalOffset/3)*i);
				UnitAppearance ua = new UnitAppearance(cell.units[i], scale_y(rotate(pos, angle_rad), scaleY));
			}			
		}
	}

	private void DrawField(float cellHeight, float cellWidth, float angle, float y_scale)
	{ 
		for (int i = 0; i < f.height + 1; i++)//draw grid horizontal lines
		{
			Vector2 start = new Vector2(fieldZeroX, fieldZeroY + cellHeight * i);
			Vector2 end = new Vector2(fieldZeroX + f.width * cellWidth, fieldZeroY + cellHeight * i);
			DrawLine(scale_y(rotate(start, angle_rad), y_scale), scale_y(rotate(end, angle_rad), y_scale));
		}
		for (int i = 0; i < f.width + 1; i++)//draw grid vertical lines
		{
			Vector2 start = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY);
			Vector2 end = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY + f.height * cellHeight);
			DrawLine(scale_y(rotate(start, angle_rad), y_scale), scale_y(rotate(end, angle_rad), y_scale));
		}
	}

	private void DrawLine(Vector2 start, Vector2 end)
	{
		GameObject g = new GameObject();
		LineRenderer lr = g.AddComponent<LineRenderer>();
		lr.startWidth = 0.05f;
		lr.endWidth = 0.05f;
		lr.material.color = Color.black;
		lr.positionCount = 2;

		lr.SetPosition(0, new Vector3(start.x, start.y, 0));
		lr.SetPosition(1, new Vector3(end.x, end.y, 0));
	}

	private Vector2 rotate(Vector2 v, float angle)
	{
		Vector2 rotated_vector2 = new Vector2();
		rotated_vector2.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
		rotated_vector2.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
		return rotated_vector2;
	}

	private Vector2 scale_y(Vector2 v, float y_scale)
	{
		return new Vector2(v.x, v.y * y_scale);
	}
}
