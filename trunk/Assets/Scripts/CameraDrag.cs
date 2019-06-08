using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
	private Vector3 dragOrigin; //Where are we moving?
	private Vector3 clickOrigin = Vector3.zero; //Where are we starting?
	private Vector3 basePos = Vector3.zero; //Where should the camera be initially?
	private float newX;
	private float newY;

	void Start()
	{
		Debug.Log("Camera start pos x = " + transform.position.x.ToString());
		Debug.Log("Camera start pos y = " + transform.position.y.ToString());
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

		/*float leftEdge = GameManager.instance.fieldZeroX + GameManager.instance.fieldSizeX / 4;
		float rightEdge = GameManager.instance.fieldZeroX + 3 * GameManager.instance.fieldSizeX / 4;
		float topEdge = GameManager.instance.fieldZeroY - GameManager.instance.fieldSizeY / 4;
		float bottomEdge = GameManager.instance.fieldZeroY - 3 * GameManager.instance.fieldSizeY / 4;*/


		float leftEdge = -10f;
		float rightEdge = 10f;
		float topEdge = 10f;
		float bottomEdge = -10f;

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
			transform.position = new Vector3(newX, newY, -10);
		}
	}
}