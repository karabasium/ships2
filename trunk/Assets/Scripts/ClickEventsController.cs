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
		bool needUpdate = false;
		if (Input.GetMouseButtonDown(0))
		{
			needUpdate = true;
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
						field.ReleaseUnitsSelection();
					}
					else //select unit
					{
						field.ReleaseUnitsSelection();
						field.AddUnitToSelectedUnits(u);
					}
				}
				else //click on Enemy unit
				{
					Unit playerUnit = field.GetLastSelectedUnit();
					Unit enemyUnit = u;					
					field.UnitAttacksUnit( playerUnit, enemyUnit );
					if (!enemyUnit.IsAlive())
					{
						field.RemoveUnit(enemyUnit);
					}
					Debug.Log("FieldAppearance: unit is alive = " + u.IsAlive());
				}
			}
			else //click in the field
			{
				if (field.GetSelectedUnits().Count > 0) //if any unit is selected
				{
					field.ChangeLastSelectedUnitPosition( Utils.GetFieldLogicalXY( mousePos2D, fa) );
				}
			}
		}

		if (needUpdate)
		{
			fa.UpdateField();
		}
	}
}
