﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	public int height;
	public int width;
	private List<Cell> cells = new List<Cell>();
	private List<Unit> units = new List<Unit>();
	private List<Unit> selectedUnits = new List<Unit>();
	public Highlight hl;
	public bool needHUDupdate = false;
	public bool unitsAnimationInProgress = false;

	public Field(int w, int h)
	{
		width = w;
		height = h;
		
		for (int row=0; row < height; row++)
		{
			for (int col=0; col < width; col++)
			{
				cells.Add(new Cell(col, row));
			}
		}
		hl = new Highlight( width, height, cells);		
	}

	public List<Cell> GetAllCells()
	{
		return cells;
	}

	public Cell getCell(int x, int y)
	{
		return cells[CellIndex(x,y)];
	}

	private int CellIndex(int x, int y)
	{
		return width * y + x;
	}

	public void AddUnit( Vector2Int positionOnField, Unit u)
	{
		Cell c = getCell(positionOnField.x, positionOnField.y);
		GameObject g = new GameObject();
		u.GameObject = g;
		units.Add(u);
		u.CellIndex = CellIndex(positionOnField.x, positionOnField.y);
		u.SetPosition(positionOnField);
		if (!c.isOccupied())
		{
			c.slotsOccupied += 1;
		}
		else
		{
			Debug.Log("ERROR! Attempt to add more than 2 ships on the same cell!");
		}
	}

	public List<Unit> GetUnits()
	{
		return units;
	}

	public void AddUnitToSelectedUnits( Unit u, bool needResetPreviouslySelectedUnits = true)
	{
		if (needResetPreviouslySelectedUnits)
		{
			ReleaseUnitsSelection();
		}

		selectedUnits.Add(u);
		hl.CreateHighlightedCellsLists(u);
		needHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public List<Unit> GetSelectedUnits()
	{
		return selectedUnits;
	}

	public void ReleaseUnitsSelection()
	{
		selectedUnits = new List<Unit>();
		hl.ResetHighlightedCellsLists();
		needHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public void UnselectUnit( Unit u)
	{
		selectedUnits.Remove(u);
		hl.ResetHighlightedCellsLists();
	}

	public void UnitAttacksUnit( Unit attacker, Unit target)
	{
		Cell c = cells[target.CellIndex];
		if (hl.canFireCells.Contains(c))
		{
			attacker.Fire(target);
			hl.ResetHighlightedCellsLists(Action.FIRE);
		}
		else
		{
			Debug.Log("Can't fire this cell");
		}
		needHUDupdate = true;
	}

	public void ChangeLastSelectedUnitPosition( Vector2Int new_pos)
	{
		if (GetSelectedUnits().Count == 0)
		{
			return;
		}

		Cell c = cells[CellIndex(new_pos.x, new_pos.y)];
		if (hl.canMoveCells.Contains(c))
		{
			Unit u = GetLastSelectedUnit();
			u.Move( new_pos, width );
			hl.ResetHighlightedCellsLists();
			hl.CreateHighlightedCellsLists(u);
		}
		else
		{
			Debug.Log("ChangeLastSelectedUnitPosition: Can't move here");
		}
	}

	public void RemoveUnit(Unit u)
	{
		units.Remove(u);
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public Unit GetLastSelectedUnit()
	{
		if (selectedUnits.Count > 0) {
			return selectedUnits[selectedUnits.Count - 1];
		}
		else
		{
			return null;
		}
	}

	public List<Unit> GetAlivePlayerUnits( Player player)
	{
		List<Unit> playerUnits = new List<Unit>();
		foreach (Unit u in units)
		{
			if (u.IsAlive() && u.Player == player)
			{
				playerUnits.Add(u);
			}
		}
		return playerUnits;
	}

	public void RefreshPlayerUnits( Player player)
	{
		foreach( Unit u in GetAlivePlayerUnits(player))
		{
			u.Refresh();
		}
	}

	public void SelectRandomUnit( Player player)
	{
		List<Unit> playerUnits = GetAlivePlayerUnits(player);
		int rndIndex = Random.Range(0, playerUnits.Count);
		AddUnitToSelectedUnits(playerUnits[rndIndex] );
		needHUDupdate = true;
	}

	public void StormMoveAllShips( )
	{
		Debug.Log("StormMoveAllShips");
		foreach (Unit u in GetAlivePlayerUnits(GameController.instance.currentPlayer))
		{
			List<Cell> cellsToMove = hl.GetHighlightedCells(u.GetPosition(), u, Action.MOVE);
			Cell cell = Utils.GetOuterMostCell(cells[ u.CellIndex ], cellsToMove);
			u.Move(new Vector2Int(cell.x, cell.y), width);
		} 
	}
}
