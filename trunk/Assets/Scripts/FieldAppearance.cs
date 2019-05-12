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
	private GameObject gridParent;
	private GameObject shipsParent;
	private List<UnitAppearance> unitsAppearances;

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

		unitsAppearances = new List<UnitAppearance>();

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();

		Debug.Log("FieldAppearance Init: angle = " + angle.ToString() + ". scaleY = " + scaleY.ToString());

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
		foreach (Unit u in field.GetSelectedUnits())
		{
			u.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		field.ReleaseUnitsSelection();
	}

	public void DrawShips()
	{
		RemoveDeadUnitsAppearances();
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
				unitsAppearances.Add(ua);
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

	public void RemoveDeadUnitsAppearances()
	{
		for (int i = unitsAppearances.Count - 1; i >= 0; i--)
		{
			UnitAppearance ua = unitsAppearances[i];
			if (!ua.u.IsAlive())
			{
				Destroy(ua.gameObject);
				Debug.Log("Unit removed");
			}
		}
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
