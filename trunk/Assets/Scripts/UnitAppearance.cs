using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance {
	public UnitAppearance( Unit u, Vector2 pos)
	{
		GameObject g = new GameObject();
		SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
		Sprite shipSprite = Resources.Load<Sprite>("Sprites/brig");
		Debug.Log(shipSprite);
		g.GetComponent<SpriteRenderer>().sprite = shipSprite;
		g.transform.localPosition = pos;

	}
}
