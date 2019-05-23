using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit u;
	private float movementAnimationSpeed = 0.2f;

	void Start()
	{

	}

	void Update()
	{
		movementAnimationSpeed = 0.2f;
		if (u.unitNeedsMovementAnimation)
		{
			Vector2 position = gameObject.transform.position;
			Vector2 destination = Utils.GetWorldPositionByLogicalXY(u.GetPosition(), GameController.instance.fa);
			if ((position.x - destination.x) < 0.1f)
			{
				transform.Translate(destination * movementAnimationSpeed * Time.deltaTime);
			}
			else
			{
				u.unitNeedsMovementAnimation = false;
				Debug.Log("Unit animation completed");
			}
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
