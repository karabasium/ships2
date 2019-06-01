using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAppearance : MonoBehaviour {
	private Highlight hl;

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
	private Color stormCellsColor;
	private Field field;

	public void Init(float angle, float scaleY, float fieldZeroX, float fieldZeroY, float cellWidth, float cellHeight, Field field)
	{
		this.angle = angle;
		this.scaleY = scaleY;
		this.fieldZeroX = fieldZeroX;
		this.fieldZeroY = fieldZeroY;
		fieldSize = new Vector2Int(field.Width, field.Height);
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		this.field = field;
		cellGameObjects = new List<GameObject>();
		highlightedCellsIndexes = new List<Vector2Int>();

		angle_rad = Mathf.PI * (angle / 180);
		viewAngle = 90 - 180 * Mathf.Asin(scaleY) / Mathf.PI;

		cellParent = new GameObject();
		cellParent.name = "highlightCellsParent";

		canMoveColor = new Color(152f / 255f, 205f / 255f, 250f / 255f);
		canFireColor = new Color(250f / 255f, 136f / 255f, 136f / 255f);
		stormCellsColor = new Color(255f / 255f, 50f / 255f, 50f / 255f);

		hl = field.Hl;
	}	

	public void CreateHighlightAppearance()
	{
		ResetHighlight();
		foreach (Cell cell in hl.CanMoveCells)
		{
			AddCellAppearance(new Vector2(cell.X, cell.Y), Action.MOVE); //should be replaced with MOVE 
		}
		foreach (Cell cell in hl.CanFireCells)
		{
			AddCellAppearance(new Vector2(cell.X, cell.Y), Action.FIRE);
		}
	}

	private void AddCellAppearance(Vector2 pos, Action type)
	{
		GameObject cellGameObject = new GameObject();
		cellGameObject.name = "cellGameObject";
		CellAppearance ca = cellGameObject.AddComponent<CellAppearance>();

		cellGameObject.transform.parent = cellParent.transform;

		cellGameObjects.Add(cellGameObject);

		ca.Init(angle, viewAngle, cellWidth, cellHeight, cellGameObject, type);
		ca.SetPosition(pos, new Vector2(fieldZeroX, fieldZeroY));

		Color defaultColor = new Color(1f, 1f, 1f);

		if (type == Action.MOVE)
		{
			ca.SetColor(canMoveColor);
		}
		else if (type == Action.FIRE)
		{
			ca.SetColor(canFireColor);
		}
		else if (type == Action.DRIFT)
		{
			ca.SetColor(stormCellsColor);
		}
		else
		{
			ca.SetColor(defaultColor);
		}
	}

	public void ResetHighlight()
	{
		foreach (GameObject go in cellGameObjects)
		{
			Destroy(go);
		}
	}
}
