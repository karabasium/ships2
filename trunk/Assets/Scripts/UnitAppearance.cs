using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit unit;

	public void PlaceUnit( Unit u, Vector2 pos)
	{
		unit = u;
		gameObject.name = "UnitAppeareance";
		sr = gameObject.AddComponent<SpriteRenderer>();
		Sprite shipSprite = Resources.Load<Sprite>("Sprites/brig");
		Debug.Log(shipSprite);
		gameObject.GetComponent<SpriteRenderer>().sprite = shipSprite;
		Debug.Log("PlaceUnit pos = " + pos.ToString());
		gameObject.transform.localPosition = pos;

		Collider2D c2d = gameObject.AddComponent<BoxCollider2D>();
		c2d.isTrigger = true;
		//gameObject.AddComponent<UnitMouseEvents>();
	
	}

	public void SetSelectedColor()
	{
		sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);
	}
}
