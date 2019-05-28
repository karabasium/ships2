using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit u;

	public Vector2 destinationPos;
	public bool startMove;

	void Start()
	{
		startMove = false;
		Debug.Log("start position = " + transform.position.ToString());
	}

	void Update()
	{
	}

	public bool Move()
	{
		Vector2 currentPosition = gameObject.transform.position;
		Debug.Log("game object name = " + gameObject.name);
		Vector2 destination = Utils.GetUnitWorldPositionByLogicalXY(u.GetPosition(), GameController.instance.fa);
		Debug.Log("unit position = " + currentPosition.ToString());
		Debug.Log("destination = " + destination.ToString());
		float tolerance = 0.05f;
		float movementAnimationSpeed = 2.0f;

		float distance = Mathf.Sqrt((currentPosition.x - destination.x) * (currentPosition.x - destination.x) + (currentPosition.y - destination.y) * (currentPosition.y - destination.y));

		Debug.Log("distance = " + distance.ToString());
		if (distance > tolerance)
		{
			Vector3 dir = new Vector3(destination.x, destination.y, 0) - transform.position;
			transform.Translate(dir.normalized * movementAnimationSpeed * Time.deltaTime);
			return false;
		}
		else
		{
			u.movementAnimationInProgress = false;
			return true;
		}
	}

	public void Init(Unit u) {
		this.u = u;
		gameObject.name = "UnitAppeareance";
		sr = gameObject.AddComponent<SpriteRenderer>();
		Sprite shipSprite = Resources.Load<Sprite>("Sprites/brig");
		gameObject.GetComponent<SpriteRenderer>().sprite = shipSprite;
		Collider2D c2d = gameObject.AddComponent<BoxCollider2D>();
		c2d.isTrigger = true;
	}

	public void PlaceUnit( Vector2 pos)
	{
		gameObject.transform.localPosition = pos;
	}

	public void ColorAsSelectedUnit()
	{
		sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);
	}

	public void ColorAsUnderFireUnit()
	{
		sr.color = new Color(1.0f, 102f/255f, 102f/255f, 1.0f);
	}

	public void ResetColor()
	{
		sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

}
