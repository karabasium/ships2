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
	private float topOffset;
	private float bottomOffset;
	private float leftOffset;
	private float rightOffset;

	void Start()
	{

	}

	public void Init(Field field, FieldAppearance fa)
	{
		rightmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width - 1, 0));
		bottommostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, 0));
		lefttopmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(0, field.Height-1));
		topmostPoint = Utils.GetWorldPositionByLogicalXY(new Vector2Int(field.Width-1, field.Height-1));
		fieldAppearanceHeight = Mathf.Abs(topmostPoint.y - bottommostPoint.y);
		fieldAppearanceWidth = Mathf.Abs(rightmostPoint.x - lefttopmostPoint.x);

		
		leftOffset = fa.LEFT_OFFSET;
		rightOffset = fa.RIGHT_OFFSET;
		topOffset = fa.TOP_OFFSET;
		bottomOffset = fa.BOTTOM_OFFSET;
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

		float leftEdge = lefttopmostPoint.x + leftOffset;
		float rightEdge = rightmostPoint.x - rightOffset;
		float topEdge = topmostPoint.y - topOffset;
		float bottomEdge = bottommostPoint.y + bottomOffset;

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