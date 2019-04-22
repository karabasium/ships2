using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit  {
	private int max_hp;
	private int hp;
	private int move_range;
	private int calm_move_range;
	private int fire_range;
	private int storm_drift_range;
	private int shots;
	private int damage_per_shot;
	private int max_shots;
	private string ship_class;
	private int player;
	private Vector2 position;
	private bool movementDone;
	private bool fireDone;
	public int cellIndex;
	public bool hasGameObject;
	public GameObject gameObject;

	public Unit( string ship_class, Vector2 startPosition, int player )
	{
		this.ship_class = ship_class;
		if (ship_class == "brig")
		{
			move_range = 5;
			calm_move_range = 1;
			storm_drift_range = 2;

			max_hp = 3;
			fire_range = 3;
			max_shots = 1;
		}
		this.player = player;
		hp = max_hp;
		position = startPosition;
		damage_per_shot = 1;
		cellIndex = -1;
		hasGameObject = false;
		refresh();
	}

	public void refresh()
	{
		movementDone = false;
		fireDone = false;
		shots = max_shots;
	}

	public void getDamage( int dmg)
	{
		hp -= dmg;
		if (hp < 0) hp = 0;
	}

	public void fire( Unit enemy, int shots_count )
	{
		for (int i = 0; i<shots_count; i++)
		{
			if (enemy.isAlive())
			{
				enemy.getDamage( damage_per_shot );
				shots -= 1;
			}
			else
			{
				break;
			}
		}
		if (shots == 0)
		{
			fireDone = true;
		}
	}

	public bool isAlive()
	{
		if (hp > 0)
		{
			return true;
		}
		return false;
	}

	public void move( Vector2 newPosition )
	{
		position = newPosition;
		movementDone = true;
	}

	public void SetPosition( Vector2 new_pos)
	{
		position = new_pos;
	}
}
