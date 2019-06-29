using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAppearance : MonoBehaviour {
	private SpriteRenderer sr;
	private SpriteRenderer mirrorSr;
	private SpriteRenderer shadowSr;
	private Dictionary<string, Sprite> sprites;
	private Dictionary<string, Sprite> spritesMirror;
	private Dictionary<string, Sprite> spritesShadows;
	private GameObject mirror;

	private string direction;
	public Unit u;
	private List<GameObject> hp_spots = new List<GameObject>();
	private bool moveStarted;

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

		if (!moveStarted)
		{
			moveStarted = true;

			string moveDirection = Utils.GetDirection(Utils.GetFieldLogicalXY(currentPosition), u.GetPosition());
			if (moveDirection == "same sell")
			{
				moveDirection = direction;
			}
			if (direction != moveDirection)
			{
				direction = moveDirection;
				SetCorrectSpritesForNewDirection(direction);
			}
		}		

		float tolerance = 0.05f;
		float movementAnimationSpeed = 2.0f;

		float distance = Mathf.Sqrt((currentPosition.x - destination.x) * (currentPosition.x - destination.x) + (currentPosition.y - destination.y) * (currentPosition.y - destination.y));

		if (distance > tolerance)
		{
			Vector3 dir = new Vector3(destination.x, destination.y, 0) - transform.position;
			transform.Translate(dir.normalized * movementAnimationSpeed * Time.deltaTime);
			SetSortingOrder();
			return false;
		}
		else
		{
			u.MovementAnimationInProgress = false;
			moveStarted = false;
			return true;
		}
	}

	public void Init(Unit u) {
		this.u = u;
		gameObject.name = "UnitAppeareance_" + u.Unit_class;
		mirror = new GameObject() { name = gameObject.name + "_mirror" };
		mirror.transform.parent = gameObject.transform;

		GameObject shadow = new GameObject { name = gameObject.name + "_shadow" };
		shadow.transform.parent = gameObject.transform;


		sr = gameObject.AddComponent<SpriteRenderer>();
		sr.sortingLayerName = "units";

		mirrorSr = mirror.AddComponent<SpriteRenderer>();
		mirrorSr.sortingLayerName = "mirrors";
		mirrorSr.color = new Color(1f, 1f, 1f, 0.6f);

		shadowSr = shadow.AddComponent<SpriteRenderer>();
		shadowSr.sortingLayerName = "shadows";
		shadowSr.color = new Color(1f, 1f, 1f, 0.85f);
		

		string spritePath = "Sprites/" + u.Unit_class;

		sprites = new Dictionary<string, Sprite>();
		spritesMirror = new Dictionary<string, Sprite>();
		spritesShadows = new Dictionary<string, Sprite>();
		if (u.Unit_class == "fort")
		{
			spritePath = "Sprites/fort";
			sprites.Add("fort", Resources.Load<Sprite>( spritePath ));
			direction = "fort";
		}
		else
		{
			
			sprites.Add("ne", Resources.Load<Sprite>(spritePath + "_ne"));
			sprites.Add("e", Resources.Load<Sprite>(spritePath + "_e"));
			sprites.Add("se", Resources.Load<Sprite>(spritePath + "_se"));
			sprites.Add("s", Resources.Load<Sprite>(spritePath + "_s"));
			sprites.Add("sw", Resources.Load<Sprite>(spritePath + "_sw"));
			sprites.Add("w", Resources.Load<Sprite>(spritePath + "_w"));
			sprites.Add("nw", Resources.Load<Sprite>(spritePath + "_nw"));
			sprites.Add("n", Resources.Load<Sprite>(spritePath + "_n"));

			spritesMirror.Add("e", Resources.Load<Sprite>(spritePath + "_e_mirror"));
			spritesMirror.Add("w", Resources.Load<Sprite>(spritePath + "_w_mirror"));
			spritesMirror.Add("n", Resources.Load<Sprite>(spritePath + "_n_mirror"));
			spritesMirror.Add("s", Resources.Load<Sprite>(spritePath + "_s_mirror"));
			spritesMirror.Add("se", Resources.Load<Sprite>(spritePath + "_se_mirror"));
			spritesMirror.Add("nw", Resources.Load<Sprite>(spritePath + "_nw_mirror"));
			spritesMirror.Add("sw", Resources.Load<Sprite>(spritePath + "_sw_mirror"));
			spritesMirror.Add("ne", Resources.Load<Sprite>(spritePath + "_ne_mirror"));

			spritesShadows.Add("e", Resources.Load<Sprite>(spritePath + "_e_shadow"));
			spritesShadows.Add("w", Resources.Load<Sprite>(spritePath + "_w_shadow"));
			spritesShadows.Add("n", Resources.Load<Sprite>(spritePath + "_n_shadow"));
			spritesShadows.Add("s", Resources.Load<Sprite>(spritePath + "_s_shadow"));
			spritesShadows.Add("se", Resources.Load<Sprite>(spritePath + "_se_shadow"));
			spritesShadows.Add("nw", Resources.Load<Sprite>(spritePath + "_nw_shadow"));
			spritesShadows.Add("ne", Resources.Load<Sprite>(spritePath + "_ne_shadow"));
			direction = "e";
		}
		
		gameObject.GetComponent<SpriteRenderer>().sprite = sprites[ direction ];


		if (spritesMirror.ContainsKey(direction))
		{
			mirrorSr.sprite = spritesMirror[direction];
		}
		else
		{
			Debug.Log("ERROR: no mirror sprite for current direction: " + direction);
		}

		if (spritesShadows.ContainsKey(direction))
		{
			shadowSr.sprite = spritesShadows[direction];
		}
		else
		{
			Debug.Log("ERROR: no shadow sprite for current direction: " + direction);
		}

		Collider2D c2d = gameObject.AddComponent<BoxCollider2D>();
		c2d.isTrigger = true;

		moveStarted = false;

		UpdateHPVisual();
	}

	private void SetCorrectSpritesForNewDirection(string direction)
	{
		if (sprites.ContainsKey(direction))
		{
			sr.sprite = sprites[direction];
		}
		else
		{
			Debug.Log("ERROR: no sprite for current direction: " + direction);
		}

		if (spritesMirror.ContainsKey(direction))
		{
			if (!mirrorSr.enabled) { mirrorSr.enabled = true; }
			
			mirrorSr.sprite = spritesMirror[direction];
		}
		else
		{
			Debug.Log("ERROR: no mirror sprite for current direction: " + direction);
			mirrorSr.enabled = false;
		}

		if (spritesShadows.ContainsKey(direction))
		{
			if (!shadowSr.enabled) { shadowSr.enabled = true; }

			shadowSr.sprite = spritesShadows[direction];
		}
		else
		{
			Debug.Log("ERROR: no mirror sprite for current direction: " + direction);
			shadowSr.enabled = false;
		}
	}

	public void Place( Vector2 pos)
	{
		gameObject.transform.localPosition = pos;
		SetSortingOrder();
	}

	public void Translate( Vector2 delta)
	{
		gameObject.transform.localPosition += new Vector3(delta.x, delta.y, 0);
		SetSortingOrder();
	}

	private void SetSortingOrder()
	{
		Vector2 topmostPoint = Utils.GetWorldTopMostPoint();
		
		sr.sortingOrder = (int)((topmostPoint.y - gameObject.transform.localPosition.y) * 100);
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
