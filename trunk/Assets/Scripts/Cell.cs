using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	//public List<Unit> units = new List<Unit>();
	public int x;
	public int y;
	public int slotsOccupied;

	public Cell(int x, int y)
	{
		this.x = x;
		this.y = y;
		slotsOccupied = 0;
	}

	public bool isOccupied()
	{
		if (slotsOccupied == 2)
		{
			return true;
		}
		return false;
	}
}
