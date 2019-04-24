using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit u;

	void Start()
	{
		gameObject.name = "UnitAppeareance";
		sr = gameObject.AddComponent<SpriteRenderer>();
		Sprite shipSprite = Resources.Load<Sprite>("Sprites/brig");
		gameObject.GetComponent<SpriteRenderer>().sprite = shipSprite;
		Collider2D c2d = gameObject.AddComponent<BoxCollider2D>();
		c2d.isTrigger = true;
		Debug.Log(shipSprite);
	}

	public void Init(Unit u) {
		this.u = u;
	}

	public void PlaceUnit( Vector2 pos)
	{
		gameObject.transform.localPosition = pos;
	}

	public void SetSelectedColor()
	{
		sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);
	}
}
