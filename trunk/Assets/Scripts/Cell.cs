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
	private CellType cellType;
	private Unit belongsToFort = null;	

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

	public CellType CellType
	{
		get
		{
			return cellType;
		}

		set
		{
			cellType = value;
		}
	}

	public Unit BelongsToFort
	{
		get
		{
			return belongsToFort;
		}

		set
		{
			belongsToFort = value;
		}
	}

	public Cell(int x, int y, CellType type)
	{
		this.X = x;
		this.Y = y;
		SlotsOccupied = 0;
		CellType = type;
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
