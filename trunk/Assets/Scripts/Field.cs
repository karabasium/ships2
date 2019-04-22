using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	public int height;
	public int width;
	private List<Cell> cells = new List<Cell>();
	private List<Unit> units = new List<Unit>();
	private Unit selectedUnit;

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
		Debug.Log("field created. cells count = " + cells.Count.ToString());
		//showCells();
	}

	public List<Cell> GetCells()
	{
		return cells;
	}

	private Cell getCell(int x, int y)
	{
		return cells[CellIndex(x,y)];
	}

	private int CellIndex(int x, int y)
	{
		return width * y + x;
	}

	public void AddUnit(int x, int y, Unit u)
	{
		Cell c = getCell(x, y);
		GameObject g = new GameObject();
		u.gameObject = g;
		units.Add(u);
		u.cellIndex = CellIndex(x, y);
		Debug.Log("AddShip: x = " + x.ToString() + "y = " + y.ToString());
		if (!c.isOccupied())
		{
			c.slotsOccupied += 1;
		}
		else
		{
			Debug.Log("ERROR! Attempt to add more than 2 ships on the same cell!");
		}
		//Debug.Log("cells:" + cells.ToString());
	}

	public List<Unit> GetUnits()
	{
		return units;
	}

	public void SetSelectedUnit(Unit u)
	{
		selectedUnit = u;
	}

	public void ChangeUnitPosition( Vector2 new_pos)
	{
		//selectedUnit.SetPosition(new_pos);
		//AddUnit((int)new_pos.x, (int)new_pos.y, selectedUnit);
		selectedUnit.cellIndex = CellIndex((int)new_pos.x, (int)new_pos.y);
		Debug.Log("ChangeUnitPosition. selected unit moved to " + ((int)new_pos.x).ToString() + ", " + ((int)new_pos.y).ToString());
	}
}
