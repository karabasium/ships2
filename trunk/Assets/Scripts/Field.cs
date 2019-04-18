using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	public int height;
	public int width;
	private List<Cell> cells = new List<Cell>();

	public Field(int h, int w)
	{
		height = h;
		width = w;
		for (int row=0; row < height; row++)
		{
			for (int col=0; col < width; col++)
			{
				cells.Add(new Cell(row, col));
			}
		}
		Debug.Log("field created. cells count = " + cells.Count.ToString());
		//showCells();
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
		return cells[width * y + x];
	}

	public void AddShip(int x, int y, Unit u)
	{
		Cell c = getCell(x, y);
		if (!c.isOccupied())
		{
			c.units.Add(u);
		}
		Debug.Log("cells:" + cells.ToString());
		showCells();
	}
}
