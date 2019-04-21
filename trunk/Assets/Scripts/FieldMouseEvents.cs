using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMouseEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
			//Debug.Log("Mouse click: " + mousePos2D.ToString());

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null)
			{
				Debug.Log("click on " + hit.collider.gameObject.name);
				SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
				sr.color = new Color(0.38f, 1.0f, 0.55f, 1.0f);
			}
		}
	}
}
