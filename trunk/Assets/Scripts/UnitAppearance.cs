using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	public Unit u;

	public Vector2 destinationPos;
	private Sprite unitSprite;

	void Start()
	{
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

	/*void AddHPVisual()
	{
		height = shipSprite.bounds.size.y;
		width = shipSprite.bounds.size.x;
		//Debug.Log("height = " + height.ToString() + ", width = " + width.ToString());
		GameObject HP = Resources.Load("Prefabs/HP") as GameObject;
		float hp_width = HP.GetComponent<SpriteRenderer>().bounds.size.x;
		float hp_space = hp_width / 8;
		float total_len = hp * hp_width + (hp - 1) * hp_space;
		Vector3 pos = gameObject.transform.position;
		float start_x = pos[0] - total_len / 2 + hp_width / 2;
		for (int i = 0; i < hp; i++)
		{
			GameObject hpObj = Instantiate(HP, new Vector3(start_x + (hp_width + hp_space) * i, pos[1] - 0.6f * height, 0), Quaternion.identity);
			hpObj.transform.parent = gameObject.transform;
			hp_spots.Add(hpObj);
		}
	}*/
}
