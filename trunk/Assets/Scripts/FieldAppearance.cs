using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAppearance : MonoBehaviour {
	public static FieldAppearance instance;
	private Field field;
	private float cellWidth;
	private float cellHeight;
	private float angle;
	private float angle_rad;
	private float scaleY;
	private float fieldZeroX;
	private float fieldZeroY;
	private List<GameObject> unitsObjects = new List<GameObject>();


	void Start()
	{
		cellWidth = 1.0f;
		cellHeight = 1.0f;
		angle = 45.0f;
		scaleY = 0.5f;
		fieldZeroX = 0;
		fieldZeroY = 0;
		angle_rad = Mathf.PI * (angle / 180);

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
			Debug.Log("Mouse click: " + mousePos2D.ToString());

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null)
			{
				Debug.Log("click on " + hit.collider.gameObject.name);
				SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
				sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);

				Unit u = hit.collider.gameObject.GetComponent<UnitAppearance>().unit;
				field.SetSelectedUnit(u);
			}
			else
			{
				Vector2 cellXY = GetCellXY(mousePos2D);
				Debug.Log("cellXY = " + cellXY.ToString());
				field.ChangeUnitPosition( cellXY );
				DrawShips();
			}
		}
	}

	private Vector2 GetCellXY( Vector2 xy)
	{
		Vector2 undistortedCellXY = rotate(scale_y(xy, 1 / scaleY), -angle_rad);
		Debug.Log("undistortedCellXY = " + undistortedCellXY.ToString());
		undistortedCellXY.x -= fieldZeroX;
		undistortedCellXY.y -= fieldZeroY;
		int new_cell_x = (int)Mathf.Floor( undistortedCellXY.x / cellWidth);
		int new_cell_y = (int)Mathf.Floor(undistortedCellXY.y / cellHeight);
		//Debug.Log("new_cell_x = " + new_cell_x.ToString());
		Debug.Log("New Cell x = " + new_cell_x.ToString() + ". New cell y =  " + new_cell_y.ToString());
		return new Vector2(new_cell_x, new_cell_y);
	}

	public void SetField( Field f)
	{
		this.field = f;
	}


	private void DrawShips()
	{
		foreach( GameObject g in unitsObjects)
		{
			Destroy(g);
		}

		foreach (Cell cell in field.GetCells())
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
				Debug.Log("DrawShips. cell.x = " + cell.x.ToString() + ", cell.y = " + cell.y.ToString());
				Vector2 pos = new Vector2(fieldZeroX + cell.x * cellWidth + 3*cellWidth / 4, fieldZeroY + cell.y * cellHeight + verticalOffset - (1*verticalOffset/3)*i);
				GameObject g = new GameObject();
				unitsObjects.Add(g);
				g.AddComponent<UnitAppearance>();
				g.GetComponent<UnitAppearance>().PlaceUnit(cell.units[i], scale_y(rotate(pos, angle_rad), scaleY));
				//UnitAppearance ua = new UnitAppearance(cell.units[i], scale_y(rotate(pos, angle_rad), scaleY));
			}			
		}
	}

	private void DrawField(float cellHeight, float cellWidth, float angle, float y_scale)
	{ 
		for (int i = 0; i < field.height + 1; i++)//draw grid horizontal lines
		{
			Vector2 start = new Vector2(fieldZeroX, fieldZeroY + cellHeight * i);
			Vector2 end = new Vector2(fieldZeroX + field.width * cellWidth, fieldZeroY + cellHeight * i);
			DrawLine(scale_y(rotate(start, angle_rad), y_scale), scale_y(rotate(end, angle_rad), y_scale));
		}
		for (int i = 0; i < field.width + 1; i++)//draw grid vertical lines
		{
			Vector2 start = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY);
			Vector2 end = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY + field.height * cellHeight);
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
