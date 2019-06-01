using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

	public static Vector2 rotate(Vector2 v, float angle)
	{
		Vector2 rotated_vector2 = new Vector2();
		rotated_vector2.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
		rotated_vector2.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
		return rotated_vector2;
	}

	public static Vector2 scale_y(Vector2 v, float y_scale)
	{
		return new Vector2(v.x, v.y * y_scale);
	}

	public static Vector2Int GetFieldLogicalXY(Vector2 xy, FieldAppearance fa)
	{
		Vector2 undistortedCellXY = Utils.rotate(Utils.scale_y(xy, 1 / fa.scaleY), -fa.angle_rad);
		undistortedCellXY.x -= fa.fieldZeroX;
		undistortedCellXY.y -= fa.fieldZeroY;
		int new_cell_x = (int)Mathf.Floor(undistortedCellXY.x / fa.cellWidth);
		int new_cell_y = (int)Mathf.Floor(undistortedCellXY.y / fa.cellHeight);
		return new Vector2Int(new_cell_x, new_cell_y);
	}

	public static Vector2 GetWorldPositionByLogicalXY( Vector2Int positionOnField, FieldAppearance fa)
	{
		Vector2 pos = new Vector2(fa.fieldZeroX + positionOnField.x * fa.cellWidth + fa.cellWidth / 2, fa.fieldZeroY + positionOnField.y * fa.cellHeight + fa.cellHeight / 2);
		return Utils.scale_y(Utils.rotate(pos, fa.angle_rad), fa.scaleY);
	}

	public static Vector2 GetUnitWorldPositionByLogicalXY(Vector2Int positionOnField, FieldAppearance fa)
	{
		Vector2 pos = new Vector2(fa.fieldZeroX + positionOnField.x * fa.cellWidth + 3 * fa.cellWidth / 4, fa.fieldZeroY + positionOnField.y * fa.cellHeight + 9 * fa.cellHeight / 10);

		return Utils.scale_y(Utils.rotate(pos, fa.angle_rad), fa.scaleY);
	}

	public static int CellIndex( Vector2Int pos, int fieldWidth)
	{
		return fieldWidth * pos.y + pos.x;
	}

	public static Cell GetOuterMostCell( Cell cellFrom, List<Cell> cellsTo) 
	{
		if (cellsTo.Count == 0)
		{
			return cellFrom;
		}

		int outermostCellIndex = 0;
		int maxDistance = Mathf.Abs(cellFrom.X - cellsTo[outermostCellIndex].X) + Mathf.Abs(cellFrom.Y - cellsTo[outermostCellIndex].Y);
		for (int i=1; i<cellsTo.Count; i++)
		{
			Cell cellTo = cellsTo[i];
			int distance = Mathf.Abs(cellFrom.X - cellTo.X) + Mathf.Abs(cellFrom.Y - cellTo.Y);
			if (distance > maxDistance)
			{
				maxDistance = distance;
				outermostCellIndex = i;
			}
		}
		return cellsTo[outermostCellIndex];
	}
}
