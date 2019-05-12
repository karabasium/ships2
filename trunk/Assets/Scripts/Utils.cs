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
}
