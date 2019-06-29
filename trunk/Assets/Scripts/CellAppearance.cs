using System.Collections;
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
	private float scaleFactor;
	private float zOffset;
	private Cell cell;
	private Material testMaterial;

	public Cell Cell
	{
		get
		{
			return cell;
		}

		set
		{
			cell = value;
		}
	}

	void Awake() {
		cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	public void Init( float gridAngle, float viewAngle, float cellWidth, float cellHeight, GameObject go, Action type, Cell cell)
	{
		width = cellHeight;
		height = cellHeight;
		grid_angle = gridAngle;
		grid_angle_rad = Mathf.PI * grid_angle / 180;
		view_angle = viewAngle;
		scaleY = Mathf.Sin(Mathf.PI * (90 - view_angle) / 180);

		zOffset = 1.0f;
		scaleFactor = 1.0f;
		if (type == Action.FIRE)
		{
			scaleFactor = 0.7f;
			zOffset = 0.8f;
		}
		else if (type == Action.HEAL)
		{
			zOffset = 1.1f;
		}

		if (cell.CellType == CellType.REEFS)
		{
			zOffset = 1.3f;
		}

		cube.transform.localScale = new Vector3(height * scaleFactor, width * scaleFactor, 0.00001f);
	
		cube.transform.Rotate(new Vector3(viewAngle, 0, gridAngle));
		float angle_rad = Mathf.PI * gridAngle / 180;

		cube.transform.parent = go.transform;
		this.Cell = cell;

		testMaterial = (Material) Resources.Load("Materials/fire");
	}
	
	public void SetPosition( Vector2 field_pos, Vector2 field_zero_pos)
	{		
		Vector2 pos = new Vector2(field_zero_pos.x + field_pos.x * width + width / 2, field_zero_pos.y + field_pos.y * height + height / 2);
		cube.transform.position = Utils.scale_y(Utils.rotate(pos, grid_angle_rad), scaleY);
		cube.transform.position = new Vector3(cube.transform.position.x, cube.transform.position.y, cube.transform.position.z + zOffset); //changing Z position for placing cube below units and grid
	}

	public void SetColor( Color color)
	{
		MeshRenderer mr = cube.GetComponent<MeshRenderer>();

		//Unity, why so ugly? 
		//just changing rendering mode to transparency
		mr.material.SetFloat("_Mode", 3);
		mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		mr.material.SetInt("_ZWrite", 0);
		mr.material.DisableKeyword("_ALPHATEST_ON");
		mr.material.EnableKeyword("_ALPHABLEND_ON");
		mr.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		mr.material.renderQueue = 3000;
		//Unity why so ugly?

		mr.material.color = color;
		
	}

	public void SetMaterial()
	{
		MeshRenderer mr = cube.GetComponent<MeshRenderer>();
		mr.material = testMaterial;
		
	}

	void Update () {
		
	}
}
