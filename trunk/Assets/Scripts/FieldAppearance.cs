using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAppearance : MonoBehaviour {
	public static FieldAppearance instance;
	private Field field;
	public float cellWidth;
	public float cellHeight;
	public float angle;
	public float angle_rad;
	public float scaleY;
	private float viewAngle;
	public float fieldZeroX;
	public float fieldZeroY;
	public HighlightAppearance hla;
	private List<GameObject> unitsObjects = new List<GameObject>();
	private GameObject gridParent;
	private GameObject shipsParent;

	void Awake()
	{
	}

	void Start()
	{

	}

	public void Init(Field f)
	{
		this.field = f;

		cellWidth = 1.0f;
		cellHeight = 1.0f;
		angle = 40.0f;
		scaleY = 0.7f;
		fieldZeroX = -2;
		fieldZeroY = -3;
		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;

		gridParent = new GameObject();
		shipsParent = new GameObject();
		gridParent.name = "grid";
		shipsParent.name = "ships";

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();

		Debug.Log("FieldAppearance Init: angle = " + angle.ToString() + ". scaleY = " + scaleY.ToString());

		//move_hl.Init(angle, scaleY, fieldZeroX, fieldZeroY, cellWidth, cellHeight, field);
		hla = new HighlightAppearance();
		hla.Init(angle, scaleY, fieldZeroX, fieldZeroY, cellWidth, cellHeight, field);
	}

	void Update()
	{
		
	}

	public void UpdateField()
	{
		DrawShips();
		DrawHighlights();
	}

	private void DrawHighlights()
	{
		hla.CreateHighlightAppearance();
	}

	public void ResetSelectedUnits()
	{
		//move_hl.ResetHighlight();		
		foreach (Unit u in field.GetSelectedUnits())
		{
			u.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		field.ReleaseUnitsSelection();
	}


	public Vector2Int GetCellLogicalXY( Vector2 xy)
	{
		Vector2 undistortedCellXY = Utils.rotate(Utils.scale_y(xy, 1 / scaleY), -angle_rad);
		undistortedCellXY.x -= fieldZeroX;
		undistortedCellXY.y -= fieldZeroY;
		int new_cell_x = (int)Mathf.Floor( undistortedCellXY.x / cellWidth);
		int new_cell_y = (int)Mathf.Floor( undistortedCellXY.y / cellHeight);
		return new Vector2Int(new_cell_x, new_cell_y);
	}

	public void RemoveDeadUnits()
	{
		List<Unit> units = field.GetUnits();
		for (int i = units.Count-1; i >= 0; i--)
		{
			Unit u = units[i];
			if (!u.isAlive())
			{
				field.RemoveUnit(u);
				Debug.Log("unit removed from units list");
				Destroy(u.gameObject);
				continue;
			}
		}
	}

	public void DrawShips()
	{
		List<int> occupiedWithOneInitCellsIndexes = new List<int>();		

		foreach (Unit unit in field.GetUnits())
		{
			Cell cell = field.GetAllCells()[unit.cellIndex];

			float verticalOffset = 3 * cellHeight / 3;

			if ( !occupiedWithOneInitCellsIndexes.Contains(unit.cellIndex) )
			{				
				verticalOffset = 9 * cellHeight / 10;
			}
			else
			{				
				verticalOffset = 1 * cellHeight / 2;
			}

			Vector2 pos = new Vector2(fieldZeroX + cell.x * cellWidth + 3*cellWidth / 4, fieldZeroY + cell.y * cellHeight + verticalOffset);
			UnitAppearance ua = unit.gameObject.GetComponent<UnitAppearance>();
			if (!ua)
			{
				ua = unit.gameObject.AddComponent<UnitAppearance>();
				ua.Init( unit );
			}
			ua.PlaceUnit(Utils.scale_y(Utils.rotate(pos, angle_rad), scaleY) );
			if (!field.GetSelectedUnits().Contains(unit))
			{
				if ( field.hl.canFireCells.Contains( cell ))
				{
					ua.ColorAsUnderFireUnit();
				}
				else
				{
					ua.ResetColor();
				}
			}
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
		g.name = "grid line";
		g.transform.parent = gridParent.transform;
		LineRenderer lr = g.AddComponent<LineRenderer>();
		lr.startWidth = 0.05f;
		lr.endWidth = 0.05f;
		lr.material.color = Color.black;
		lr.positionCount = 2;

		lr.SetPosition(0, new Vector3(start.x, start.y, 0));
		lr.SetPosition(1, new Vector3(end.x, end.y, 0));
	}
}
