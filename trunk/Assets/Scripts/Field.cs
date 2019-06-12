using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
	private int height;
	private int width;
	private List<Cell> cells = new List<Cell>();
	private List<Unit> units = new List<Unit>();
	private List<Unit> selectedUnits = new List<Unit>();
	private Highlight hl;
	private bool needHUDupdate = false;
	private bool unitsAnimationInProgress = false;
	

	public Field(int w, int h)
	{
		Width = w;
		Height = h;
		
		for (int row=0; row < Height; row++)
		{
			for (int col=0; col < Width; col++)
			{
				cells.Add(new Cell(col, row, CellType.SEA));
			}
		}
		Hl = new Highlight( Width, Height, cells);		
	}


	public void AddUnit( Vector2Int positionOnField, Unit u)
	{
		Cell c = GetCell(positionOnField.x, positionOnField.y);
		GameObject g = new GameObject();
		u.GameObject = g;
		units.Add(u);
		u.cell = GetCell(positionOnField.x, positionOnField.y);
		u.SetPosition(positionOnField);
		if (!c.isOccupied())
		{
			if (u.Unit_class == "fort")
			{
				c.SlotsOccupied = 2;
			}
			else
			{
				c.SlotsOccupied += 1;
			}
		}
		else
		{
			Debug.Log("ERROR! Attempt to add more than 2 ships on the same cell!");
		}
		if (u.Unit_class == "fort")
		{
			foreach (Cell cell in hl.GetCellsAreaForAction(u.Position, u, Action.HEAL))
			{
				cell.BelongsToFort = u;
			}
		}
	}

	public void AddUnit( Unit u)
	{
		Cell c = GetCell(u.Position.x, u.Position.y);
		u.cell = c;

		GameObject g = new GameObject();
		u.GameObject = g;
		units.Add(u);		

		if (!c.isOccupied())
		{
			if (u.Unit_class == "fort")
			{
				c.SlotsOccupied = 2;
			}
			else
			{
				c.SlotsOccupied += 1;
			}
		}
		else
		{
			Debug.Log("ERROR! Attempt to add more than 2 ships on the same cell!");
		}
		if (u.Unit_class == "fort")
		{
			foreach (Cell cell in hl.GetCellsAreaForAction(u.Position, u, Action.HEAL))
			{
				cell.BelongsToFort = u;
			}
		}
	}

	public void ResetFortCells()
	{
		foreach(Cell c in cells)
		{
			c.BelongsToFort = null;
		}
	}

	public void AssignFortToFortAdjacentCells()
	{
		foreach(Unit u in units)
		{
			if (u.Unit_class == "fort")
			{
				foreach(Cell c in hl.GetCellsAreaForAction(u.Position, u, Action.HEAL))
				{
					c.BelongsToFort = u;
				}
			}
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
		Hl.CreateHighlightedCellsLists(u);
		NeedHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public List<Unit> GetSelectedUnits()
	{
		return selectedUnits;
	}

	public void ReleaseUnitsSelection()
	{
		selectedUnits = new List<Unit>();
		Hl.ResetHighlightedCellsLists();
		NeedHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public void UnselectUnit( Unit u)
	{
		selectedUnits.Remove(u);
		Hl.ResetHighlightedCellsLists();
	}

	public void UnitAttacksUnit( Unit attacker, Unit target)
	{
		Cell c = target.cell;
		if (Hl.CanFireCells.Contains(c))
		{
			attacker.Fire(target);
			if (!target.IsAlive())
			{
				units.Remove(target);
			}
			Hl.ResetHighlightedCellsLists(Action.FIRE);
		}
		else
		{
			Debug.Log("Can't fire this cell");
		}
		NeedHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
	}

	public void RemoveDeadUnits()
	{
		for (int i = units.Count - 1; i >= 0; i--)
		{
			Unit u = units[i];
			if (!u.IsAlive())
			{
				units.Remove(u);
			}
		}
	}

	public void ChangeLastSelectedUnitPosition( Vector2Int new_pos)
	{
		if (GetSelectedUnits().Count == 0)
		{
			return;
		}

		Cell c = cells[CellIndex(new_pos.x, new_pos.y)];
		if (Hl.CanMoveCells.Contains(c))
		{
			Unit u = GetLastSelectedUnit();
			u.Move( c );
			Hl.ResetHighlightedCellsLists();
			Hl.CreateHighlightedCellsLists(u);
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
		NeedHUDupdate = true;
	}

	public void StormMoveAllShips( )
	{
		Debug.Log("StormMoveAllShips");
		foreach (Unit u in GetAlivePlayerUnits(GameController.instance.currentPlayer))
		{
			List<Cell> cellsToMove = Hl.GetCellsAreaForAction(u.GetPosition(), u, Action.MOVE);
			Cell cell = Utils.GetOuterMostCell( u.cell, cellsToMove);
			u.Move( cell );
		}
	}

	private void KillUnitsDestroyedByStorm()
	{
		foreach(Unit unit in units)
		{
			Cell unitCell = GetCell(unit.Position.x, unit.Position.y);
			if (unitCell.CellType == CellType.REEFS)
			{
				RemoveUnit(unit);
			}
		}
	}

	public List<Cell> GetAllCells()
	{
		return cells;
	}

	public Cell GetCell(int x, int y)
	{
		return cells[CellIndex(x, y)];
	}

	private int CellIndex(int x, int y)
	{
		return Width * y + x;
	}

	public int Height
	{
		get
		{
			return height;
		}

		set
		{
			height = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}

		set
		{
			width = value;
		}
	}

	public Highlight Hl
	{
		get
		{
			return hl;
		}

		set
		{
			hl = value;
		}
	}

	public bool UnitsAnimationInProgress
	{
		get
		{
			return unitsAnimationInProgress;
		}

		set
		{
			unitsAnimationInProgress = value;
		}
	}

	public bool NeedHUDupdate
	{
		get
		{
			return needHUDupdate;
		}

		set
		{
			needHUDupdate = value;
		}
	}
}
