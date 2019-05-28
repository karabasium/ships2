﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit  {
	private readonly int max_hp;
	public int hp;
	public readonly int move_range;
	public readonly int calm_move_range;
	public readonly int fire_range;
	public readonly int storm_drift_range;
	public int shots;
	private readonly int damage_per_shot;
	private readonly int max_shots;
	private readonly string ship_class;
	public Player player;
	private Vector2Int position;
	public bool movementDone;
	public bool fireDone;
	public int cellIndex;
	public bool hasGameObject;
	public GameObject gameObject;
	public float HIT_PROBABILITY = 0.5f;
	public bool movementAnimationInProgress = false;

	public Unit( string ship_class, Player player )
	{
		this.ship_class = ship_class;
		if (ship_class == "brig")
		{
			move_range = 5;
			calm_move_range = 2;
			storm_drift_range = 3;

			max_hp = 1;
			fire_range = 3;
			max_shots = 1;
		}
		this.player = player;
		hp = max_hp;

		position = Vector2Int.zero;

		damage_per_shot = 1;
		cellIndex = -1;
		hasGameObject = false;
		HIT_PROBABILITY = 0.5f;
		Refresh();
	}

	public void Refresh()
	{
		movementDone = false;
		fireDone = false;
		shots = max_shots;
	}

	public void GetDamage( int dmg )
	{
		hp -= dmg;
		if (hp < 0) {
			hp = 0;
			Debug.Log("Unit destroyed");
		}
	}

	public void Fire( Unit enemy  )
	{
		Debug.Log("UNIT: fire!");
		for (int i = 0; i<shots; i++)
		{
			float rnd = Random.Range(0.0f, 1.0f);
			//rnd = 0f; //always hit
			//rnd = 1f; //always miss
			//Debug.Log("rnd = " + rnd.ToString());
			//Debug.Log("HIT_PROBABILITY = " + HIT_PROBABILITY.ToString());
			//Debug.Log("rnd < HIT_PROBABILITY = " + (rnd < HIT_PROBABILITY).ToString());

			if (enemy.IsAlive())
			{
				if (rnd < HIT_PROBABILITY)
				{
					enemy.GetDamage(damage_per_shot);
					Debug.Log("Hit!");
				}
				else
				{
					Debug.Log("Miss!");
				}
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

	public bool IsAlive()
	{
		if (hp > 0)
		{
			return true;
		}
		return false;
	}

	public void Move( Vector2Int newPosition, int fildWidth )
	{
		position = newPosition;
		cellIndex = Utils.CellIndex(newPosition, fildWidth);
		movementDone = true;
		movementAnimationInProgress = true;
		if ( GameController.instance.gameState != GAME_STATE.ANIMATION_IN_PROGRESS)
		{
			GameController.instance.gameState = GAME_STATE.ANIMATION_IN_PROGRESS;
			Debug.Log("Current game state is " + GameController.instance.gameState.ToString());
		}		
	}

	public void SetPosition( Vector2Int new_pos)
	{
		position = new_pos;
	}

	public Vector2Int GetPosition()
	{
		return position;
	}
}
