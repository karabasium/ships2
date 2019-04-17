using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance {
	public UnitAppearance( Unit u)
	{
		GameObject g = new GameObject();
		SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
		Sprite shipSprite = Resources.Load<Sprite>("Sprites/brig");
		Debug.Log(shipSprite);
		g.GetComponent<SpriteRenderer>().sprite = shipSprite;
		g.transform.localPosition = new Vector2(1, 3);

	}
}
