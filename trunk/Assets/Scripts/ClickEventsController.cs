using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEventsController : MonoBehaviour {
	private Field field;
	private FieldAppearance fa;

	// Use this for initialization
	void Start () {
		
	}

	public void Init( FieldAppearance fa, Field f)
	{
		this.fa = fa;
		field = f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

			if (hit.collider != null) // if clicked on unit
			{
				Unit u = hit.collider.gameObject.GetComponent<UnitAppearance>().u;
				Debug.Log("click on unit");

				if (field.GetLastSelectedUnit() == null || u.player == field.GetLastSelectedUnit().player) //click on Player's unit
				{

					SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
					sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);

					if (field.GetSelectedUnits().Contains(u)) //unselect unit if clicked again
					{
						fa.ResetSelectedUnits();
					}
					else //select unit
					{
						fa.ResetSelectedUnits();
						field.AddUnitToSelectedUnits(u);
						fa.MakeNeccessaryHighlights(u);
						fa.DrawShips();
					}
				}
				else //click on Enemy unit
				{
					Unit playerUnit = field.GetLastSelectedUnit();
					Unit enemyUnit = u;
					playerUnit.fire(enemyUnit); //fire on enemy unit
					Debug.Log("FieldAppearance: unit is alive = " + u.isAlive());
					fa.RemoveDeadUnits();
					fa.MakeNeccessaryHighlights(playerUnit);
					fa.DrawShips();
				}
			}
			else //click in the field
			{
				if (field.GetSelectedUnits().Count > 0) //if any unit is selected
				{
					Vector2Int cellXY = fa.GetCellLogicalXY(mousePos2D);
					List<Vector2Int> availableCells = fa.move_hl.GetHighlightedCellsIndexes();

					if (availableCells.Contains(cellXY))
					{

						if (cellXY.x <= field.width && cellXY.y <= field.height && cellXY.x >= 0 && cellXY.y >= 0) //if desired location is valid field cell
						{
							field.ChangeLastSelectedUnitPosition(cellXY); //move selected unit in desired cell
																		  //ResetSelectedUnits();
							field.GetLastSelectedUnit().movementDone = true;
							fa.MakeNeccessaryHighlights(field.GetLastSelectedUnit());
							fa.DrawShips();
						}
						else
						{
							//Debug.Log("WARNING: Attempt to place unit outside the battlefield");
						}
					}
					else
					{
						Debug.Log("This cell is not available for movement");
					}
				}
			}
		}
	}
}
