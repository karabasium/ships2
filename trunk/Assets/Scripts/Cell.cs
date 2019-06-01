using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	//public List<Unit> units = new List<Unit>();
	private int x;
	private int y;
	private int slotsOccupied;
	private bool isUnderFire = false;

	public int X
	{
		get
		{
			return x;
		}

		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}

		set
		{
			y = value;
		}
	}

	public int SlotsOccupied
	{
		get
		{
			return slotsOccupied;
		}

		set
		{
			slotsOccupied = value;
		}
	}

	public bool IsUnderFire
	{
		get
		{
			return isUnderFire;
		}

		set
		{
			isUnderFire = value;
		}
	}

	public Cell(int x, int y)
	{
		this.X = x;
		this.Y = y;
		SlotsOccupied = 0;
	}

	public bool isOccupied()
	{
		if (SlotsOccupied == 2)
		{
			return true;
		}
		return false;
	}
}
