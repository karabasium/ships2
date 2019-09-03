using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
	private GameObject backgroundTilesHolder;
	private List<GameObject> backgroundTiles;
	private Sprite tileSprite;

	public void Init(float width, float height, Vector2 BoundingFrameTopLeftPoint_position)
	{
		backgroundTilesHolder = new GameObject() { name = "Background2" };

		tileSprite = Resources.Load<Sprite>("Sprites/sea");

		float tileHeight = tileSprite.bounds.size.y;
		float tileWidth = tileSprite.bounds.size.x;


		int x_tiles_count = (int)Mathf.Ceil(width / tileWidth);
		int y_tiles_count = (int)Mathf.Ceil(height / tileHeight);

		//Debug.Log("Background needs x = " + x_tiles_count.ToString() + ". y = " + y_tiles_count.ToString());
		
		for (int i=0; i<x_tiles_count; i++)
		{
			for (int j = 0; j < y_tiles_count; j++)
			{
				Vector2 center = new Vector2(BoundingFrameTopLeftPoint_position.x + i * tileWidth + tileWidth / 2, BoundingFrameTopLeftPoint_position.y - j*tileHeight - tileHeight / 2);
				CreateTile(center);
			}
		}
	}

	private GameObject CreateTile( Vector2 pos )
	{
		GameObject tile = new GameObject { name = "sea tile" };
		SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
		sr.sprite = tileSprite;
		sr.sortingLayerName = "Background";
		tile.transform.parent = backgroundTilesHolder.transform;
		tile.transform.position += new Vector3(pos.x, pos.y, 2);

		return tile;
	}
}
