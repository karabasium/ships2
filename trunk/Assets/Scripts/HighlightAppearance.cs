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
	private FieldAppearance fa;
	private GameObject cellGameObjectParent;

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

		canMoveColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
		canFireColor = new Color(250f / 255f, 136f / 255f, 136f / 255f);
		stormCellsColor = new Color(255f / 255f, 50f / 255f, 50f / 255f);

		hl = field.Hl;
		fa = GameController.instance.fa;

		cellGameObjectParent = new GameObject();
		cellGameObjectParent.name = "cellGameObject";
	}	

	public void CreateHighlightAppearance()
	{
		ResetHighlight();
		foreach (Cell cell in hl.CanMoveCells)
		{
			cellGameObjects.Add( fa.AddCellAppearance(new Vector2(cell.X, cell.Y), Action.MOVE, cell) );
			cellGameObjects[cellGameObjects.Count - 1].transform.parent = cellGameObjectParent.transform;
		}
		foreach (Cell cell in hl.CanFireCells)
		{
			cellGameObjects.Add(fa.AddCellAppearance(new Vector2(cell.X, cell.Y), Action.FIRE, cell));
			cellGameObjects[cellGameObjects.Count - 1].transform.parent = cellGameObjectParent.transform;
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
