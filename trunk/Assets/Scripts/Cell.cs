using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	public List<Unit> units = new List<Unit>();
	public int x;
	public int y;

	public Cell(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public bool isOccupied()
	{
		if (units.Count == 2)
		{
			return true;
		}
		return false;
	}
}
