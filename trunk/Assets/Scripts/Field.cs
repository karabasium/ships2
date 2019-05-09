using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	public int height;
	public int width;
	private List<Cell> cells = new List<Cell>();
	private List<Unit> units = new List<Unit>();
	private List<Unit> selectedUnits = new List<Unit>();

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
		//Debug.Log("field created. cells count = " + cells.Count.ToString());
		//showCells();
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
	}

	public List<Unit> GetSelectedUnits()
	{
		return selectedUnits;
	}

	public void ReleaseUnitsSelection()
	{
		selectedUnits = new List<Unit>();
	}

	public void UnselectUnit( Unit u)
	{
		selectedUnits.Remove(u);
	}

	public void ChangeLastSelectedUnitPosition( Vector2Int new_pos)
	{
		selectedUnits[selectedUnits.Count-1].cellIndex = CellIndex((int)new_pos.x, (int)new_pos.y);
		selectedUnits[selectedUnits.Count - 1].SetPosition( new_pos );
	}

	public void RemoveUnit(Unit u)
	{
		units.Remove(u);
		//u = null;
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
