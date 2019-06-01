using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit u;
	private List<GameObject> hp_spots = new List<GameObject>();

	void Update()
	{
		if (u.Hp != hp_spots.Count)
		{
			UpdateHPVisual();
		}
	}

	public bool Move()
	{
		Vector2 currentPosition = gameObject.transform.position;
		Vector2 destination = Utils.GetUnitWorldPositionByLogicalXY(u.GetPosition(), GameController.instance.fa);
		float tolerance = 0.05f;
		float movementAnimationSpeed = 2.0f;

		float distance = Mathf.Sqrt((currentPosition.x - destination.x) * (currentPosition.x - destination.x) + (currentPosition.y - destination.y) * (currentPosition.y - destination.y));

		if (distance > tolerance)
		{
			Vector3 dir = new Vector3(destination.x, destination.y, 0) - transform.position;
			transform.Translate(dir.normalized * movementAnimationSpeed * Time.deltaTime);
			return false;
		}
		else
		{
			u.MovementAnimationInProgress = false;
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
		UpdateHPVisual();
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

	private void UpdateHPVisual()
	{
		RemoveVisualHP();

		float height = sr.bounds.size.y;
	
		float scaleFactor = 0.12f;
		float hp_width = scaleFactor;
		float hp_space = hp_width / 8;
		float total_len = u.Hp * hp_width + (u.Hp - 1) * hp_space;
		Vector3 pos = gameObject.transform.position;
		float start_x = pos[0] - total_len / 2 + hp_width / 2;
		for (int i = 0; i < u.Hp; i++)
		{
			GameObject hpObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			hpObj.transform.Rotate(new Vector3(90, 0, 0));
			hpObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			hpObj.transform.position = new Vector3(start_x + (hp_width + hp_space) * i, pos[1] - 0.6f * height, 0);
			hpObj.transform.parent = gameObject.transform;
			if (u.Player == Player.PLAYER_1)
			{
				hpObj.GetComponent<MeshRenderer>().material.color = new Color(2.0f/255f, 254f/255.0f, 38f/255f, 1.0f);
			}
			else
			{
				hpObj.GetComponent<MeshRenderer>().material.color = new Color(254.0f/255f, 2f / 255f, 49f / 255f, 1.0f);
			}
			hp_spots.Add(hpObj);
		}
	}

	public void RemoveVisualHP()
	{
		for (int i = hp_spots.Count - 1; i >= 0; i--)
		{
			GameObject gameObjToRemove = hp_spots[i];
			hp_spots.Remove(gameObjToRemove);
			Destroy(gameObjToRemove);
		}
	}
}
