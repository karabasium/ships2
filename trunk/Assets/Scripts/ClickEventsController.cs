using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEventsController : MonoBehaviour {
	private Field field;
	private FieldAppearance fa;
	private HUD hud;
	private float distance;
	bool dragging = false;
	private GameObject dragGameObject;

	void Start () {
		
	}

	public void Init( FieldAppearance fa, Field f)
	{
		this.fa = fa;
		field = f;
		hud = GameObject.Find("HUD").GetComponent<HUD>();
	}

	private Cell GetClosestCell(Vector2 pos)
	{
		if (!field.IsValidPosition(Utils.GetFieldLogicalXY(pos)))
		{
			return null;
		}

		Cell closestCell = field.GetAllCells()[0];

		float distance = (pos - Utils.GetCellVisualCenter(closestCell)).magnitude;
		foreach( Cell cell in field.GetAllCells())
		{
			Vector2 cellCenter = Utils.GetCellVisualCenter(cell);
			float currentDistance = (pos - cellCenter).magnitude;
			if (currentDistance < distance)
			{
				closestCell = cell;
				distance = currentDistance;
			}
		}
		return closestCell;
	}

	private void HandleGameClicks()
	{
		if (Input.GetMouseButtonDown(0))
		{

			if (EventSystem.current.IsPointerOverGameObject()) // prevent interacting with objects below ui elements
			{
				return;
			}

			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			Player currentPlayer = GameController.instance.currentPlayer;

			Cell cell = GetClosestCell(mousePos2D);

			if (cell == null)
			{
				return;
			}



			List<Unit> unitsInCell = Utils.GetUnitsInCell(cell);
			List<Unit> currentPlayerUnits = new List<Unit>();
			List<Unit> enemyPlayerUnits = new List<Unit>();
			foreach (Unit u in unitsInCell)
			{
				if (u.Player == currentPlayer)
				{
					currentPlayerUnits.Add(u);
				}
				else if (u.Player != currentPlayer)
				{
					enemyPlayerUnits.Add(u);
				}
			}
			Unit selectedUnit = field.GetLastSelectedUnit();

			if (selectedUnit == null) //nothing selected yet
			{
				if (unitsInCell.Count > 0)
				{
					Unit unitToSelect = unitsInCell[0];
					field.AddUnitToSelectedUnits(unitToSelect); //select unit
				}
			}
			else //some unit is already selected
			{
				if (selectedUnit.Player == currentPlayer) // if selected unit is current player's unit
				{
					if (unitsInCell.Count == 0) // if no units in clicked cell
					{
						if (field.CanPerformActionOnCell(selectedUnit, cell, Action.MOVE)) // if possible to move there
						{
							field.TryMove(selectedUnit, Utils.GetFieldLogicalXY(mousePos2D));  // then try move unit there
						}
						else
						{
							field.ReleaseUnitsSelection();
						}
					}
					else // if there are some units in cell
					{
						if (unitsInCell.Count == 1 && unitsInCell[0].Player == currentPlayer) // if cell is not empty and only 1 PLAYER's unit there
						{
							//code below should have ui selector for action

							//if (field.CanPerformActionOnCell(selectedUnit, cell, Action.MOVE)) // if possible to move there
							//{
							//	field.TryMove(selectedUnit, Utils.GetFieldLogicalXY(mousePos2D)); // then move 
							//}
							//else //if impossible to move
							if (field.GetSelectedUnits().Contains(unitsInCell[0]))
							{
								field.UnselectUnit(unitsInCell[0]);
							}
							else
							{
								//if both units don't have defined weather than select them all to have possibility to define weather for all units
								if (unitsInCell[0].weather.currentWeatherType == Weather_type.UNDEFINED && selectedUnit.weather.currentWeatherType == Weather_type.UNDEFINED && !unitsInCell[0].MovementDone && !selectedUnit.MovementDone)
								{
									field.AddUnitToSelectedUnits(unitsInCell[0], false); // select unit in this cell
								}
								else //unselect current unit and select clicked unit
								{
									field.UnselectUnit(selectedUnit);
									field.AddUnitToSelectedUnits(unitsInCell[0], false);
								}
							}
						}
						else if (unitsInCell.Count == 1 && unitsInCell[0].Player != currentPlayer) // if cell is not empty and only 1 ENEMY's unit there
						{
							if (field.CanPerformActionOnCell(selectedUnit, cell, Action.FIRE)) // if possible to fire
							{
								field.UnitAttacksUnit(selectedUnit, unitsInCell[0]); //then fire
							}
							else //if impossible to fire
							{
								if (field.CanPerformActionOnCell(selectedUnit, cell, Action.MOVE)) //if possible to move
								{
									field.TryMove(selectedUnit, Utils.GetFieldLogicalXY(mousePos2D)); // then move
								}
								else // if impossible to move nor fire
								{
									field.AddUnitToSelectedUnits(unitsInCell[0]); //select ENEMY unit in this cell
								}
							}
						}
						else if (unitsInCell.Count == 2 && currentPlayerUnits.Count == 2)
						{
							field.AddUnitToSelectedUnits(unitsInCell[0]);

						}
						else if (unitsInCell.Count == 2 && enemyPlayerUnits.Count == 2)
						{
							if (field.CanPerformActionOnCell(selectedUnit, cell, Action.FIRE)) // if possible to fire
							{
								field.UnitAttacksUnit(selectedUnit, unitsInCell[0]); //then fire
							}
							else //if impossible to fire
							{
								field.AddUnitToSelectedUnits(unitsInCell[0]); //select ENEMY unit in this cell
							}
						}
						else if (unitsInCell.Count == 2 && enemyPlayerUnits.Count == 1 && currentPlayerUnits.Count == 1)
						{
							if (field.CanPerformActionOnCell(selectedUnit, cell, Action.FIRE)) // if possible to fire
							{
								field.UnitAttacksUnit(selectedUnit, enemyPlayerUnits[0]); //then fire
							}
							else //if impossible to fire
							{
								field.AddUnitToSelectedUnits(currentPlayerUnits[0]); //select ENEMY unit in this cell
							}
						}
					}
				}
				else //if selected  unit is enemy unit
				{
					if (unitsInCell.Count > 0) //if there are any units in clicked cell
					{
						field.AddUnitToSelectedUnits(unitsInCell[0]);
					}
					else //if clicked cell is empty
					{
						field.ReleaseUnitsSelection(); //release selection
					}
				}
			}
		}
	}

	private void HandleEditorClicks()
	{
		if (Input.GetMouseButtonUp(0))
			{
				Debug.Log("GetMouseButtonUp");
			}
			if (Input.GetMouseButtonDown(0))
			{
				Debug.Log("GetMouseButtonDown");
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

				RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

				if (hit.collider == null)
				{
					return;
				}

				if (hit.collider.gameObject != null)
				{
					if (!dragging)
					{
						dragGameObject = hit.collider.gameObject;
						distance = Vector3.Distance(dragGameObject.transform.position, Camera.main.transform.position);
						dragging = true;
						Debug.Log("start drag");
					}
				}
				else
				{
					AddOrRemoveObjects(Utils.GetFieldLogicalXY(mousePos2D));
				}
			}
			else if (Input.GetMouseButtonUp(0) && dragging)
			{
				dragging = false;
				Debug.Log("stop drag");
				fa.UpdateLandCells();
				fa.DrawCells();
			}
			if (dragging)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 rayPoint = ray.GetPoint(distance);
				dragGameObject.transform.position = rayPoint;
			}
	}

	void Update()
	{
		if (GameController.instance.Mode == Mode.EDITOR)
		{
			HandleEditorClicks();
		}
		else
		{
			HandleGameClicks();
		}
	}


	void AddOrRemoveObjects(Vector2Int cellXY)
	{
			

					if (cellXY.x <= field.Width && cellXY.y <= field.Height && cellXY.x >= 0 && cellXY.y >= 0)
					{
						Cell c = field.GetCell(cellXY.x, cellXY.y);
						string objectType = hud.GetObjectTypeToPlace();
						if (objectType == "land")
						{
							if (c.SlotsOccupied == 0)
							{
								c.CellType = CellType.LAND;
							}
						}
						else if (objectType == "reefs")
						{
							if (c.SlotsOccupied == 0)
							{
								c.CellType = CellType.REEFS;
							}
						}
						else if (objectType == "erase")
						{
							c.CellType = CellType.SEA;
							if (c.SlotsOccupied > 0)
							{
								foreach (Unit u in field.GetUnits())
								{
									if (u.cell == c)
									{
										u.Hp = 0;
									}
								}
								field.RemoveDeadUnits();
							}
						}
						else if (objectType == "brig")
						{
							if (c.SlotsOccupied < 2)
							{
								Unit u = new Unit("brig", hud.GetPlayer())
								{
									Position = new Vector2Int(c.X, c.Y)
								};
								field.AddUnit(u);
							}
						}
						else if (objectType == "fort")
						{
							if (c.SlotsOccupied == 0)
							{

								Unit u = new Unit("fort", hud.GetPlayer())
								{
									Position = new Vector2Int(c.X, c.Y)
								};
								field.AddUnit(u);
							}
						}
						fa.DrawCells();
						fa.DrawShips();
					}				
			
		
	}
}
