using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils {
	public static FieldAppearance fa;
	public static Field field;

	public static Vector2 rotate(Vector2 v, float angle)
	{
		Vector2 rotated_vector2 = new Vector2();
		rotated_vector2.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
		rotated_vector2.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
		return rotated_vector2;
	}

	public static List<Unit> GetUnitsInCell(Cell cell)
	{
		List<Unit> units = new List<Unit>();
		foreach(Unit u in field.GetUnits())
		{
			if (u.cell == cell)
			{
				units.Add(u);
			}
		}
		return units;
	}

	public static Vector2 GetWorldTopMostPoint()
	{
		return GetWorldPositionByLogicalXY(new Vector2Int(field.Width - 1, field.Height - 1));
	}

	public static Vector2 GetCellVisualCenter( Cell cell)
	{
		return GetWorldPositionByLogicalXY(new Vector2Int(cell.X, cell.Y));
	}

	public static Vector2 scale_y(Vector2 v, float y_scale)
	{
		return new Vector2(v.x, v.y * y_scale);
	}

	public static Vector2Int GetFieldLogicalXY(Vector2 xy)
	{
		Vector2 undistortedCellXY = Utils.rotate(Utils.scale_y(xy, 1 / fa.scaleY), -fa.angle_rad);
		undistortedCellXY.x -= fa.fieldZeroX;
		undistortedCellXY.y -= fa.fieldZeroY;
		int new_cell_x = (int)Mathf.Floor(undistortedCellXY.x / fa.cellWidth);
		int new_cell_y = (int)Mathf.Floor(undistortedCellXY.y / fa.cellHeight);
		return new Vector2Int(new_cell_x, new_cell_y);
	}

	public static Vector2 GetWorldPositionByLogicalXY( Vector2Int positionOnField)
	{
		Vector2 pos = new Vector2(fa.fieldZeroX + positionOnField.x * fa.cellWidth + fa.cellWidth / 2, fa.fieldZeroY + positionOnField.y * fa.cellHeight + fa.cellHeight / 2);
		return Utils.scale_y(Utils.rotate(pos, fa.angle_rad), fa.scaleY);
	}

	public static Vector2 GetUnitWorldPositionByLogicalXY(Vector2Int positionOnField, FieldAppearance fa)
	{
		Vector2 pos = new Vector2(fa.fieldZeroX + positionOnField.x * fa.cellWidth + 1 * fa.cellWidth / 2, fa.fieldZeroY + positionOnField.y * fa.cellHeight + 1 * fa.cellHeight / 2);

		return Utils.scale_y(Utils.rotate(pos, fa.angle_rad), fa.scaleY);
	}

	public static int CellIndex( Vector2Int pos, int fieldWidth)
	{
		return fieldWidth * pos.y + pos.x;
	}

	public static GameObject GetUnitGameObject(Unit u)
	{
		return fa.GetUnitAppearanceInCell(u.cell).gameObject;
	}

	public static Cell GetOuterMostCell(Cell cellFrom, List<Cell> cellsTo)
	{
		if (cellsTo.Count == 0)
		{
			return cellFrom;
		}

		Dictionary<Cell, int> distances = new Dictionary<Cell, int>();

		for (int i = 0; i < cellsTo.Count; i++)
		{
			Cell cellTo = cellsTo[i];
			int distance = Mathf.Abs(cellFrom.X - cellTo.X) + Mathf.Abs(cellFrom.Y - cellTo.Y);
			distances.Add(cellTo, distance);
		}
		int j = 0;
		foreach (KeyValuePair<Cell, int> d in distances.OrderBy(key => key.Value))
		{
			if (d.Key.CellType == CellType.REEFS)
			{
				return d.Key;
			}
			if (j == distances.Keys.Count - 1)
			{
				return d.Key;
			}
			j++;
		}
		return cellFrom;
	}

	public static string GetDirection( Vector2Int xyFrom, Vector2Int xyTo)
	{
		Vector2Int diff = xyTo - xyFrom;
		//Debug.Log("GetDirection diff = " + diff.ToString());
		if (diff.x > 0 && diff.y > 0)
		{
			return "n";
		}
		else if (diff.x > 0 && diff.y == 0)
		{
			return "ne";
		}
		else if (diff.x > 0 && diff.y < 0)
		{
			return "e";
		}
		else if (diff.x == 0 && diff.y < 0)
		{
			return "se";
		}
		else if (diff.x < 0 && diff.y < 0)
		{
			return "s";
		}
		else if (diff.x < 0 && diff.y == 0)
		{
			return "sw";
		}
		else if (diff.x < 0 && diff.y > 0)
		{
			return "w";
		}
		else if (diff.x == 0 && diff.y > 0)
		{
			return "nw";
		}
		return "same sell";
	}
}
