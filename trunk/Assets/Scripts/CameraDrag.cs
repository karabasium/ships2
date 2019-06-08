using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
	private Vector3 dragOrigin; //Where are we moving?
	private Vector3 clickOrigin = Vector3.zero; //Where are we starting?
	private Vector3 basePos = Vector3.zero; //Where should the camera be initially?
	private float newX;
	private float newY;
	private Vector2 rightmostPoint;
	private Vector2 bottommostPoint;
	private Vector2 topmostPoint;
	private Vector2 lefttopmostPoint;
	private float fieldAppearanceWidth;
	private float fieldAppearanceHeight;

	void Start()
	{

	}

	public void Init(Field field, FieldAppearance fa)
	{
		rightmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width - 1, 0), fa);
		bottommostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, 0), fa);
		lefttopmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, field.Height-1), fa);
		topmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width-1, field.Height-1), fa);
		fieldAppearanceHeight = Mathf.Abs(topmostPoint.y - bottommostPoint.y);
		fieldAppearanceWidth = Mathf.Abs(rightmostPoint.x - lefttopmostPoint.x);
	}


	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (clickOrigin == Vector3.zero)
			{
				clickOrigin = Input.mousePosition;
				basePos = transform.position;
			}
			dragOrigin = Input.mousePosition;
		}

		if (!Input.GetMouseButton(0))
		{
			clickOrigin = Vector3.zero;
			return;
		}



		float leftEdge = lefttopmostPoint.x + fieldAppearanceWidth / 4;
		float rightEdge = rightmostPoint.x - fieldAppearanceWidth / 4;
		float topEdge = topmostPoint.y - fieldAppearanceHeight / 4;
		float bottomEdge = bottommostPoint.y + fieldAppearanceHeight / 4;

		if (transform.position.x >= leftEdge && transform.position.x <= rightEdge && transform.position.y <= topEdge && transform.position.y >= bottomEdge)
		{
			newX = basePos.x + ((clickOrigin.x - dragOrigin.x) * .01f);
			newY = basePos.y + ((clickOrigin.y - dragOrigin.y) * .01f);
			if (newX < leftEdge)
			{
				newX = leftEdge + 0.01f;
			}
			else if (newX > rightEdge)
			{
				newX = rightEdge - 0.01f;
			}
			if (newY > topEdge)
			{
				newY = topEdge - 0.01f;
			}
			else if (newY < bottomEdge)
			{
				newY = bottomEdge + 0.01f;
			}
			transform.position = new Vector3(newX, newY, transform.position.z);
		}
	}
}