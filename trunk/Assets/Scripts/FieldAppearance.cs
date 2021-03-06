﻿using System.Collections.Generic;
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
	private GameObject fieldObjectsParent;
	private GameObject gridParent;
	private GameObject shipsParent;
	private List<UnitAppearance> unitsAppearances;
	private List<CellAppearance> allCellAppearances;
	private GameObject reefsParentObject;
	private GameObject landParentObject;
	private Color canMoveColor;
	private Color canFireColor;
	private Color stormCellsColor;
	private Vector2 topLeftPoint_position;
	private float width;
	private float height;
	public float LEFT_OFFSET = 4;
	public float RIGHT_OFFSET = 4;
	public float TOP_OFFSET = 4;
	public float BOTTOM_OFFSET = 3;
	private List<GameObject> landObjects = new List<GameObject>();

	public float Width
	{
		get
		{
			return width;
		}

		set
		{
			width = value;
		}
	}

	public float Height
	{
		get
		{
			return height;
		}

		set
		{
			height = value;
		}
	}

	public Vector2 TopLeftPoint_position
	{
		get
		{
			return topLeftPoint_position;
		}

		set
		{
			topLeftPoint_position = value;
		}
	}

	public List<GameObject> LandObjects
	{
		get
		{
			return landObjects;
		}

		set
		{
			landObjects = value;
		}
	}

	void Awake()
	{
	}

	void Start()
	{

	}

	public void Init(Field f)
	{
		this.field = f;

		reefsParentObject = new GameObject() { name = "reefsParent" };
		landParentObject = new GameObject() { name = "landParent" };

		allCellAppearances = new List<CellAppearance>();

		canMoveColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0.9f);
		canFireColor = new Color(250f / 255f, 136f / 255f, 136f / 255f, 0.9f);
		stormCellsColor = new Color(255f / 255f, 50f / 255f, 50f / 255f, 0.5f);

		cellWidth = 1.0f;
		cellHeight = 1.0f;
		angle = 55.0f;
		scaleY = 0.7f;
		//angle = 0.0f;  //for debug
		//scaleY = 1.0f;  //for debug
		fieldZeroX = -4;
		fieldZeroY = -5;
		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;

		fieldObjectsParent = new GameObject() { name = "fieldObjectsParent" };

		gridParent = new GameObject() { name = "gridParent" };
		gridParent.transform.parent = fieldObjectsParent.transform;
		shipsParent = new GameObject() { name = "shipsParent" };
		shipsParent.transform.parent = fieldObjectsParent.transform;

		unitsAppearances = new List<UnitAppearance>();
		

		DrawField(cellHeight, cellWidth, angle, scaleY);
		DrawShips();

		hla = gameObject.AddComponent<HighlightAppearance>();
		hla.Init(angle, scaleY, fieldZeroX, fieldZeroY, cellWidth, cellHeight, field);

		Vector2 topMostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width - 1, field.Height - 1));
		Vector2 bottomMostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, 0));
		Vector2 leftMostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, field.Height - 1));
		Vector2 rightMostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width - 1, 0));

		Height = Mathf.Abs(topMostPoint.y - bottomMostPoint.y);
		Width = Mathf.Abs(leftMostPoint.x - rightMostPoint.x);

		Vector2 BoundingFrameTopLeftPoint_position = new Vector2(leftMostPoint.x, topMostPoint.y);
		Debug.Log("BoundingFrameTopLeftPoint_position = " + BoundingFrameTopLeftPoint_position.ToString());

		GameObject backGameObject = new GameObject() { name = "back game object" };
		Background back = backGameObject.AddComponent<Background>();
		back.Init(Width, Height, BoundingFrameTopLeftPoint_position, LEFT_OFFSET, RIGHT_OFFSET, TOP_OFFSET, BOTTOM_OFFSET);

		CreateFortCells();

		for (int i=0; i<GameController.instance.levelData.landAppearancesInfo.Count; i++)
		{
			Vector2 distortedPos = Utils.DistortPosition( GameController.instance.levelData.landAppearancesInfo[i].Position );
			string name = GameController.instance.levelData.landAppearancesInfo[i].Name;

			CreateLandObject(name, distortedPos);
		}
		LandObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("land"));
		UpdateLandCells();
	}

	public GameObject CreateLandObject(string spriteName, Vector2 position)
	{
		GameObject g = new GameObject
		{
			name = spriteName
		};
		g.transform.position = position;
		g.transform.parent = landParentObject.transform;
		SpriteRenderer sp = g.AddComponent<SpriteRenderer>();
		sp.sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
		sp.sortingLayerName = "Field";
		PolygonCollider2D pgc = g.AddComponent<PolygonCollider2D>();
		pgc.isTrigger = true;
		g.tag = "land";
		LandObjects.Add(g);
		return g;
	}

	public void UpdateLandCells()
	{
		for (int x = 0; x < field.Width; x++)
		{
			for (int y = 0; y < field.Height; y++)
			{
				RaycastHit2D hit = Physics2D.Raycast(Utils.GetWorldPositionByLogicalXY(new Vector2Int(x, y)), Vector2.zero);
				Cell cell = field.GetCell(x, y);
				if (hit.collider != null)
				{
					if (hit.collider.name.ToLower().Contains("land"))
					{
						Debug.Log("hit.collider = " + hit.collider.ToString());
						cell.CellType = CellType.LAND;
					}
				}
				else
				{
					if (cell.CellType == CellType.LAND)
					{
						cell.CellType = CellType.SEA;
					}
				}
			}
		}
	}

	void Update()
	{
		if (GameController.instance.gameState == GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE)
		{
			Debug.Log("Field Appearances need to update");
			UpdateField();
			GameController.instance.ChangeState(GAME_STATE.NOTHING_HAPPENS);
			return;
		}
		else if (GameController.instance.gameState == GAME_STATE.ANIMATION_IN_PROGRESS)
		{
			List<Unit> unitsToAnimate = GetUnitsNeedAnimation();
			Debug.Log("units to animate count = " + unitsToAnimate.Count.ToString());
			if (unitsToAnimate.Count > 0)
			{
				bool animationCompleted = true;

				foreach (Unit u in unitsToAnimate)
				{
					animationCompleted = FindUnitAppearance(u).Move();
				}

				if (animationCompleted)
				{
					GameController.instance.ChangeState(GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE);
				}
			}
		}
	}

	private List<Unit> GetUnitsNeedAnimation()
	{
		List<Unit> units = new List<Unit>();
		foreach (Unit u in field.GetUnits())
		{
			//Debug.Log("u.GetPosition() = " + u.GetPosition().ToString() + "u.previousPosition = " + u.previousPosition.ToString());
			if (u.MovementAnimationInProgress)
			{
				units.Add(u);
			}
		}
		return units;
	}

	public UnitAppearance FindUnitAppearance( Unit u)
	{
		foreach(UnitAppearance ua in unitsAppearances)
		{
			if (ua.u == u)
			{
				return ua;
			}
		}
		return null;
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
			u.GameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		field.ReleaseUnitsSelection();
	}

	public UnitAppearance GetUnitAppearanceInCell( Cell cell)
	{
		Vector2Int cellPosition = new Vector2Int(cell.X, cell.Y);
		foreach(UnitAppearance ua in unitsAppearances)
		{
			if (ua.u.Position == cellPosition)
			{
				return ua;
			}
		}
		return null;
	}

	public void DrawShips()
	{
		RemoveDeadUnitsAppearances();
		List<Cell> occupiedWithOneUnitCells = new List<Cell>();		

		foreach (Unit unit in field.GetUnits())
		{
			Cell cell = unit.cell;

			float verticalOffset = 0;
			float horizontalOffset = 0.5f;

			if ( !occupiedWithOneUnitCells.Contains(cell) )
			{
				//verticalOffset = 9 * cellHeight / 10;
				verticalOffset = 0.5f;
			}
			else //if cell already occupied by another ship
			{
				UnitAppearance another_ship = GetUnitAppearanceInCell(cell);
				if (another_ship == null)
				{
					Debug.Log("ERROR! No another ship appearance found for occupied cell");
					verticalOffset = 0.5f;
				}
				else
				{
					another_ship.Translate(new Vector2(0.1f * cellWidth, -0.15f * cellHeight));
					verticalOffset = 0.75f;
					horizontalOffset = 0.75f;
				}
			}
						
			Vector2 pos = new Vector2(fieldZeroX + cell.X * cellWidth + horizontalOffset, fieldZeroY + cell.Y * cellHeight + verticalOffset);
			UnitAppearance ua = unit.GameObject.GetComponent<UnitAppearance>();
			if (!ua)
			{
				ua = unit.GameObject.AddComponent<UnitAppearance>();
				unit.GameObject.transform.parent = shipsParent.transform;
				unitsAppearances.Add(ua);
				ua.Init( unit );
			}
			ua.Place(Utils.scale_y(Utils.rotate(pos, angle_rad), scaleY) );
			if (GameController.instance.Mode != Mode.EDITOR)
			{
				if (!field.GetSelectedUnits().Contains(unit)) //if unit is not selected
				{
					if (field.Hl.CanFireCells.Contains(cell) && unit.Player != GameController.instance.currentPlayer && field.GetLastSelectedUnit().Player == GameController.instance.currentPlayer)
					{
						ua.ColorAsUnderFireUnit();
					}
					else
					{
						ua.ResetColor();
					}
				}
				else //if unit is selected
				{
					ua.VisualizeAsSelected();
				}
			}
			occupiedWithOneUnitCells.Add(unit.cell);
		}
	}

	public void RemoveDeadUnitsAppearances()
	{
		for (int i = unitsAppearances.Count - 1; i >= 0; i--)
		{
			UnitAppearance ua = unitsAppearances[i];
			if (!ua.u.IsAlive())
			{
				unitsAppearances.Remove(ua);
				Destroy(ua.gameObject);
				Debug.Log("Unit removed");
			}
		}
	}

	public void RemoveCellAppearances()
	{
		for (int i = allCellAppearances.Count - 1; i >= 0; i--)
		{
			CellAppearance ca = allCellAppearances[i];
			allCellAppearances.Remove(ca);
			Destroy(ca.gameObject);
		}
	}

	private void CreateFortCells()
	{
		foreach (Cell cell in field.GetAllCells())
		{
			Unit fort = cell.BelongsToFort;
			if (fort == null)
			{
				continue;
			}

			GameObject fortObject = GetGameObjectForUnit(fort);
			GameObject cellGameObject = new GameObject
			{
				name = "fortCell"
			};
			CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();

			cellGameObject.transform.parent = fortObject.transform;

			ca.Init(angle, viewAngle, cellWidth, cellHeight, cellGameObject, Action.HEAL, cell);
			ca.SetPosition(new Vector2(cell.X, cell.Y), new Vector2(fieldZeroX, fieldZeroY));
			ca.SetColor(new Color(158f / 255f, 250f / 255f, 160f / 255f));
		}
	}

	private GameObject GetGameObjectForUnit( Unit u)
	{
		foreach( UnitAppearance ua in unitsAppearances)
		{
			if (ua.u == u)
			{
				return ua.gameObject;
			}
		}
		return null;
	}

	private void DrawField(float cellHeight, float cellWidth, float angle, float y_scale)
	{ 
		for (int i = 0; i < field.Height + 1; i++)//draw grid horizontal lines
		{
			Vector2 start = new Vector2(fieldZeroX, fieldZeroY + cellHeight * i);
			Vector2 end = new Vector2(fieldZeroX + field.Width * cellWidth, fieldZeroY + cellHeight * i);
			DrawLine(Utils.scale_y(Utils.rotate(start, angle_rad), y_scale), Utils.scale_y(Utils.rotate(end, angle_rad), y_scale));
		}
		for (int i = 0; i < field.Width + 1; i++)//draw grid vertical lines
		{
			Vector2 start = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY);
			Vector2 end = new Vector2(fieldZeroX + cellWidth * i, fieldZeroY + field.Height * cellHeight);
			DrawLine(Utils.scale_y(Utils.rotate(start, angle_rad), y_scale), Utils.scale_y(Utils.rotate(end, angle_rad), y_scale));
		}
		GameObject landCellsParent = new GameObject();
		DrawCells();
	}

	public void DrawCells()
	{
		RemoveCellAppearances();
		foreach (Cell cell in field.GetAllCells())
		{
			if (cell.CellType != CellType.SEA)
			{
				AddCellAppearance(Action.NONE, cell);
			}
		}
	}

	public void Clear()
	{
		Destroy(fieldObjectsParent);
		Destroy(GameObject.Find("highlightCellsParent"));
	}

	private void DrawLine(Vector2 start, Vector2 end)
	{
		GameObject g = new GameObject();
		g.name = "grid line";
		g.transform.parent = gridParent.transform;
		LineRenderer lr = g.AddComponent<LineRenderer>();
		lr.startWidth = 0.01f;
		lr.endWidth = 0.01f;
		lr.material.color = Color.black;
		lr.positionCount = 2;

		lr.SetPosition(0, new Vector3(start.x, start.y, 0));
		lr.SetPosition(1, new Vector3(end.x, end.y, 0));

		lr.sortingLayerName = "Field";
	}

	public GameObject AddCellAppearance(Action type, Cell cell)
	{
		GameObject cellGameObject = new GameObject();
		CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();

		ca.Init(angle, viewAngle, cellWidth, cellHeight, cellGameObject, type, cell);
		ca.SetPosition(new Vector2(cell.X, cell.Y), new Vector2(fieldZeroX, fieldZeroY));

		Color defaultColor = new Color(1f, 1f, 1f);

		if (type == Action.MOVE)
		{
			ca.SetColor(canMoveColor);			
			cellGameObject.name = "move";
		}
		else if (type == Action.FIRE)
		{
			ca.SetColor(canFireColor);
			//ca.SetMaterial();
			cellGameObject.name = "fire";
		}
		else if (type == Action.DRIFT)
		{
			ca.SetColor(stormCellsColor);
		}
		else if (type == Action.NONE)
		{
			if (cell.CellType == CellType.LAND)
			{

				ca.SetColor(new Color(245f / 255f, 175f / 255f, 88f / 255f, 0f));
				ca.gameObject.transform.parent = landParentObject.transform;
			}
			else if (cell.CellType == CellType.REEFS)
			{
				ca.SetColor(new Color(255f / 255f, 0f / 255f, 0f / 255f));
				ca.gameObject.transform.parent = reefsParentObject.transform;
			}
			allCellAppearances.Add(ca);
		}
		else
		{
			ca.SetColor(defaultColor);
		}		
		return cellGameObject;
	}
}
