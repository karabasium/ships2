﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAppearance : MonoBehaviour {
	private GameObject cube;
	private float width;
	private float height;
	private float grid_angle;
	private float grid_angle_rad;
	private float view_angle;
	private float scaleY;

	void Awake() {
		cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	public void Init( float gridAngle, float viewAngle, float cellWidth, float cellHeight, GameObject go)
	{
		width = cellHeight;
		height = cellHeight;
		grid_angle = gridAngle;
		grid_angle_rad = Mathf.PI * grid_angle / 180;
		view_angle = viewAngle;
		scaleY = Mathf.Sin(Mathf.PI * (90 - view_angle) / 180);

		cube.transform.localScale = new Vector3(height, width, 0.00001f);
		Debug.Log("CA Init: viewAngle = " + viewAngle.ToString() + ". gridAngle = " + gridAngle.ToString());
		cube.transform.Rotate(new Vector3(viewAngle, 0, gridAngle));
		float angle_rad = Mathf.PI * gridAngle / 180;

		cube.transform.parent = go.transform;
	}
	
	public void SetPosition( Vector2 field_pos, Vector2 field_zero_pos )
	{
		Vector2 pos = new Vector2(field_zero_pos.x + field_pos.x * width + width / 2, field_zero_pos.y + field_pos.y * height + height / 2);
		Debug.Log("CA setPosition: pos = " + pos.ToString());
		cube.transform.position = Utils.scale_y(Utils.rotate(pos, grid_angle_rad), scaleY);
		cube.transform.position = new Vector3(cube.transform.position.x, cube.transform.position.y, cube.transform.position.z + 1); //changing Z position for placing cube below units and grid
	}

	// Update is called once per frame
	void Update () {
		
	}
}
