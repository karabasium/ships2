using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	public int height;
	public int width;
	private List<Cell> cells = new List<Cell>();
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

	private void showCells()
	{
		for (int i=0; i<cells.Count; i++)
		{
			Debug.Log(cells[i].units.Count);
		}
	}

	private Cell getCell(int x, int y)
	{
		Debug.Log("getCell. height = " + height.ToString());
		int i = width * y + x;
		Debug.Log("getCell i="+i.ToString());
		return cells[i];
	}

	public void AddShip(int x, int y, Unit u)
	{
		Cell c = getCell(x, y);
		Debug.Log("AddShip: x = " + x.ToString() + "y = " + y.ToString());
		if (!c.isOccupied())
		{
			c.units.Add(u);
		}
		else
		{
			Debug.Log("ERROR! Attempt to add more than 2 ships on the same cell!");
		}
		//Debug.Log("cells:" + cells.ToString());
		//showCells();
	}

	public void SetSelectedUnit(Unit u)
	{
		selectedUnit = u;
	}

	public void ChangeUnitPosition( Vector2 new_pos)
	{
		//selectedUnit.SetPosition(new_pos);
		AddShip((int)new_pos.x, (int)new_pos.y, selectedUnit);
		Debug.Log("ChangeUnitPosition. selected unit moved to " + ((int)new_pos.x).ToString() + ", " + ((int)new_pos.y).ToString());
	}
}
