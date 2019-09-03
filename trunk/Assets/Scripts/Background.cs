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

		CameraDrag cd = GameController.instance.cd;

		float LeftOffset = 4;
		float RightOffset = 4;
		float TopOffset = 4;
		float BottomOffset = 3;

		float frameWidth = width + LeftOffset + RightOffset;
		float frameHeight = height + TopOffset + BottomOffset;


		int x_tiles_count = (int)Mathf.Ceil(frameWidth / tileWidth);
		int y_tiles_count = (int)Mathf.Ceil(frameHeight / tileHeight);

		//Debug.Log("Background needs x = " + x_tiles_count.ToString() + ". y = " + y_tiles_count.ToString());
		
		for (int i=0; i<x_tiles_count; i++)
		{
			for (int j = 0; j < y_tiles_count; j++)
			{
				Vector2 center = new Vector2(BoundingFrameTopLeftPoint_position.x - LeftOffset + i * tileWidth + tileWidth / 2, BoundingFrameTopLeftPoint_position.y + TopOffset - j*tileHeight - tileHeight / 2);
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
