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
	private float viewAngle;
	private float fieldZeroX;
	private float fieldZeroY;
	private List<GameObject> unitsObjects = new List<GameObject>();


	void Start()
	{
		cellWidth = 1.0f;
		cellHeight = 1.0f;
		angle = 45.0f;
		scaleY = 0.5f;
		fieldZeroX = -3;
		fieldZeroY = -2;
		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;

		AddCellAppearance( new Vector2(0,0));
		AddCellAppearance(new Vector2(1, 1));
		AddCellAppearance(new Vector2(2, 1));

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();
	}

	private void AddCellAppearance( Vector2 pos)
	{
		GameObject cellGameObject = new GameObject();
		CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();
		ca.Init(angle, viewAngle, cellWidth, cellHeight);
		ca.SetPosition( pos, new Vector2(fieldZeroX, fieldZeroY));
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
			//Debug.Log("Mouse click: " + mousePos2D.ToString());

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null)
			{
				SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
				sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);

				Unit u = hit.collider.gameObject.GetComponent<UnitAppearance>().u;

				if (field.GetSelectedUnits().Contains(u)) //unselect unit if clicked again
				{
					Debug.Log("This unit is already selected");
					field.UnselectUnit(u);
					u.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				}
				else
				{
					ResetSelectedUnitsHighlight();
					field.ReleaseUnitsSelection();

					field.AddUnitToSelectedUnits(u);
				}
			}
			else
			{
				if (field.GetSelectedUnits().Count > 0)
				{
					Vector2 cellXY = GetCellLogicalXY(mousePos2D);
					//Debug.Log("cellXY = " + cellXY.ToString());
					if (cellXY.x <= field.width && cellXY.y <= field.height && cellXY.x >= 0 && cellXY.y >= 0)
					{
						field.ChangeLastSelectedUnitPosition(cellXY);
						ResetSelectedUnitsHighlight();
						field.ReleaseUnitsSelection();
						DrawShips();
					}
					else
					{
						Debug.Log("WARNING: Attempt to place unit outside the battlefield");
					}
				}
			}
		}
	}

	private void ResetSelectedUnitsHighlight()
	{
		foreach (Unit u in field.GetSelectedUnits())
		{
			u.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			Debug.Log("reset selection");
		}
	}

	private Vector2 GetCellLogicalXY( Vector2 xy)
	{
		Vector2 undistortedCellXY = Utils.rotate(Utils.scale_y(xy, 1 / scaleY), -angle_rad);
		undistortedCellXY.x -= fieldZeroX;
		undistortedCellXY.y -= fieldZeroY;
		int new_cell_x = (int)Mathf.Floor( undistortedCellXY.x / cellWidth);
		int new_cell_y = (int)Mathf.Floor( undistortedCellXY.y / cellHeight);
		return new Vector2(new_cell_x, new_cell_y);
	}

	public void SetField( Field f)
	{
		this.field = f;
	}


	private void DrawShips()
	{
		List<int> occupiedWithOneInitCellsIndexes = new List<int>();

		foreach (Unit unit in field.GetUnits())
		{
			Cell cell = field.GetCells()[unit.cellIndex];

			float verticalOffset = 3 * cellHeight / 3;

			if ( !occupiedWithOneInitCellsIndexes.Contains(unit.cellIndex) )
			{				
				Debug.Log("No slots occupied");
				verticalOffset = 9 * cellHeight / 10;
			}
			else
			{				
				verticalOffset = 1 * cellHeight / 2;
				Debug.Log("1 slot occupied");
			}

			Debug.Log("DrawShips. cell.x = " + cell.x.ToString() + ", cell.y = " + cell.y.ToString());
			Vector2 pos = new Vector2(fieldZeroX + cell.x * cellWidth + 3*cellWidth / 4, fieldZeroY + cell.y * cellHeight + verticalOffset);
			UnitAppearance ua = unit.gameObject.GetComponent<UnitAppearance>();
			if (!ua)
			{
				ua = unit.gameObject.AddComponent<UnitAppearance>();
				ua.Init( unit );
			}
			ua.PlaceUnit(Utils.scale_y(Utils.rotate(pos, angle_rad), scaleY) );
			occupiedWithOneInitCellsIndexes.Add(unit.cellIndex);
		}
	}

	private Vector2 GetWorldPositionByLogicalXY( int field_x, int field_y)
	{
		Vector2 pos = new Vector2(fieldZeroX + field_x * cellWidth + cellWidth/2, fieldZeroY + field_y  * cellHeight+cellHeight/2);
		return Utils.scale_y(Utils.rotate(pos, angle_rad), scaleY);
	}

	private void DrawField(float cellHeight, float cellWidth, float angle, float y_scale)
	{ 
		for (int i = 0; i < field.height + 1; i++)//draw grid horizontal lines
		{
			Vector2 start = new Vector2(fieldZeroX, fieldZeroY + cellHeight * i);
			Vector2 end = new Vector2(fieldZeroX + field.width * cellWidth, fieldZeroY + cellHeight * i);
			DrawLine(Utils.scale_y(Utils.rotate(start, angle_rad), y_scale), Utils.scale_y(Utils.rotate(end, angle_rad), y_scale));
		}
		for (int i = 0; i < field.width + 1; i++)//draw grid vertical lines
		{
			Vector2 start = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY);
			Vector2 end = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY + field.height * cellHeight);
			DrawLine(Utils.scale_y(Utils.rotate(start, angle_rad), y_scale), Utils.scale_y(Utils.rotate(end, angle_rad), y_scale));
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
}
