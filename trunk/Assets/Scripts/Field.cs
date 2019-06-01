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

	public Field(int w, int h)
	{
		Width = w;
		Height = h;
		
		for (int row=0; row < Height; row++)
		{
			for (int col=0; col < Width; col++)
			{
				cells.Add(new Cell(col, row));
			}
		}
		Hl = new Highlight( Width, Height, cells);		
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
		return Width * y + x;
	}

	public void AddUnit( Vector2Int positionOnField, Unit u)
	{
		Cell c = getCell(positionOnField.x, positionOnField.y);
		GameObject g = new GameObject();
		u.GameObject = g;
		units.Add(u);
		u.CellIndex = CellIndex(positionOnField.x, positionOnField.y);
		u.SetPosition(positionOnField);
		if (!c.isOccupied())
		{
			c.SlotsOccupied += 1;
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
		Cell c = cells[target.CellIndex];
		if (Hl.CanFireCells.Contains(c))
		{
			attacker.Fire(target);
			Hl.ResetHighlightedCellsLists(Action.FIRE);
		}
		else
		{
			Debug.Log("Can't fire this cell");
		}
		NeedHUDupdate = true;
		GameController.instance.gameState = GAME_STATE.FIELD_APPEARANCE_NEED_TO_UPDATE;
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
			u.Move( new_pos, Width );
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
			List<Cell> cellsToMove = Hl.GetHighlightedCells(u.GetPosition(), u, Action.MOVE);
			Cell cell = Utils.GetOuterMostCell(cells[ u.CellIndex ], cellsToMove);
			u.Move(new Vector2Int(cell.X, cell.Y), Width);
		} 
	}
}
