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
		u.gameObject = g;
		units.Add(u);
		u.cellIndex = CellIndex(positionOnField.x, positionOnField.y);
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

	public void SetSelectedUnits( List<Unit> units)
	{
		selectedUnits = units;
	}

	public void AddUnitToSelectedUnits( Unit u)
	{
		selectedUnits.Add(u);
		hl.CreateHighlightedCellsLists(u);
	}

	public List<Unit> GetSelectedUnits()
	{
		return selectedUnits;
	}

	public void ReleaseUnitsSelection()
	{
		selectedUnits = new List<Unit>();
		hl.ResetHighlightedCellsLists();
	}

	public void UnselectUnit( Unit u)
	{
		selectedUnits.Remove(u);
		hl.ResetHighlightedCellsLists();
	}

	public void UnitAttacksUnit( Unit attacker, Unit target)
	{
		attacker.Fire(target);
		hl.ResetHighlightedCellsLists("fire");
	}

	public void ChangeLastSelectedUnitPosition( Vector2Int new_pos)
	{
		Unit u = GetLastSelectedUnit();
		u.cellIndex = CellIndex( new_pos.x, new_pos.y);
		u.SetPosition( new_pos );
		u.movementDone = true;
		hl.ResetHighlightedCellsLists();
		hl.CreateHighlightedCellsLists(u);
	}

	public void RemoveUnit(Unit u)
	{
		units.Remove(u);
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
}
