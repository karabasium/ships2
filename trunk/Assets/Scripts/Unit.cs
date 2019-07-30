using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit  {

	private readonly int max_hp;
	private int hp;
	private int shots;
	private int damage_per_shot;
	private readonly int max_shots;
	private readonly float hitProbability = 0.5f;

	private Vector2Int position;
	private Player player;

	private bool movementDone;
	private bool fireDone;
	private Cell _cell;
	private readonly int move_range;
	private readonly int calm_move_range;
	private readonly int fire_range;
	private readonly int storm_drift_range;
	private readonly int heal_range;

	private GameObject gameObject;

	private bool movementAnimationInProgress;
	private readonly string ship_class;
	private List<Cell> fortCells;

	public Weather weather;
	
	public Unit( string ship_class, Player player )
	{
		this.ship_class = ship_class;
		heal_range = 0;
		if (ship_class == "brig")
		{
			move_range = 5;
			calm_move_range = 2;
			storm_drift_range = 1;

			max_hp = 4;
			fire_range = 3;
			max_shots = 3;
		}
		else if (ship_class == "three_deck_battleship")
		{
			move_range = 5;
			calm_move_range = 2;
			storm_drift_range = 3;

			max_hp = 1;
			fire_range = 3;
			max_shots = 1;
		}
		else if( ship_class == "fort")
		{
			move_range = 0;
			calm_move_range = 0;
			storm_drift_range = 0;

			max_hp = 5;
			fire_range = 4;
			max_shots = 2;
			heal_range = 1;
		}
		this.Player = player;
		Hp = max_hp;

		Position = Vector2Int.zero;

		damage_per_shot = 1;
		cell = null;
		hitProbability = GameController.instance.HIT_PROBABILITY;
		movementAnimationInProgress = false;
		weather = new Weather();
		Refresh();
	}

	public void Refresh()
	{
		MovementDone = false;
		FireDone = false;
		Shots = max_shots;
		weather.currentWeatherType = Weather_type.UNDEFINED;
	}

	public void GetDamage( int dmg )
	{
		Hp -= dmg;
		if (Hp < 0) {
			Hp = 0;
			Debug.Log("Unit destroyed");
		}
	}

	public void Fire( Unit enemy  )
	{
		Debug.Log("UNIT: fire!");
		damage_per_shot = GameController.instance.hud.GetShotsCountUserSelected();

		float rnd = Random.Range(0.0f, 1.0f);
		//rnd = 0f; //always hit
		//rnd = 1f; //always miss
		//Debug.Log("rnd = " + rnd.ToString());
		//Debug.Log("HIT_PROBABILITY = " + HIT_PROBABILITY.ToString());
		//Debug.Log("rnd < HIT_PROBABILITY = " + (rnd < HIT_PROBABILITY).ToString());

		if (enemy.IsAlive())
		{
			if (rnd < hitProbability)
			{
				enemy.GetDamage(damage_per_shot);
				Debug.Log("Hit!");
			}
			else
			{
				Debug.Log("Miss!");
			}
			Shots -= damage_per_shot;
		}


		if (Shots == 0)
		{
			FireDone = true;
		}
	}

	public bool IsAlive()
	{
		if (Hp > 0)
		{
			return true;
		}
		return false;
	}

	public void Move( Cell cellToMove )
	{
		Position = new Vector2Int(cellToMove.X, cellToMove.Y);
		cell.SlotsOccupied -= 1;
		cell = cellToMove;
		cell.SlotsOccupied += 1;
		MovementDone = true;
		MovementAnimationInProgress = true;
		if ( GameController.instance.gameState != GAME_STATE.ANIMATION_IN_PROGRESS)
		{
			GameController.instance.ChangeState(GAME_STATE.ANIMATION_IN_PROGRESS);
			Debug.Log("Current game state is " + GameController.instance.gameState.ToString());
		}
		weather.currentWeatherType = Weather_type.UNDEFINED;
	}

	public void SetPosition( Vector2Int new_pos)
	{
		Position = new_pos;
	}

	public Vector2Int GetPosition()
	{
		return Position;
	}

	public int Shots
	{
		get
		{
			return shots;
		}

		set
		{
			shots = value;
		}
	}

	public int Hp
	{
		get
		{
			return hp;
		}

		set
		{
			hp = value;
		}
	}

	public Vector2Int Position
	{
		get
		{
			return position;
		}

		set
		{
			position = value;
		}
	}

	public Player Player
	{
		get
		{
			return player;
		}

		set
		{
			player = value;
		}
	}

	public bool MovementDone
	{
		get
		{
			return movementDone;
		}

		set
		{
			movementDone = value;
		}
	}

	public bool FireDone
	{
		get
		{
			return fireDone;
		}

		set
		{
			fireDone = value;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return gameObject;
		}

		set
		{
			gameObject = value;
		}
	}

	public int Move_range
	{
		get
		{
			return move_range;
		}
	}

	public int Calm_move_range
	{
		get
		{
			return calm_move_range;
		}
	}

	public int Storm_drift_range
	{
		get
		{
			return storm_drift_range;
		}
	}

	public int Fire_range
	{
		get
		{
			return fire_range;
		}
	}

	public bool MovementAnimationInProgress
	{
		get
		{
			return movementAnimationInProgress;
		}

		set
		{
			bool prevMovementAnimationInProgress = movementAnimationInProgress;
			movementAnimationInProgress = value;
			if (prevMovementAnimationInProgress != movementAnimationInProgress && movementAnimationInProgress == false)
			{
				if (cell.CellType == CellType.REEFS)
				{
					Hp = 0;
					Debug.Log("ship ran on the reefs");
				}
			}
		}
	}

	public string Unit_class
	{
		get
		{
			return ship_class;
		}
	}

	public List<Cell> FortCells
	{
		get
		{
			return fortCells;
		}

		set
		{
			fortCells = value;
		}
	}

	public int Heal_range
	{
		get
		{
			return heal_range;
		}
	}

	public int Max_hp
	{
		get
		{
			return max_hp;
		}
	}

	public Cell cell
	{
		get
		{
			return _cell;
		}

		set
		{
			_cell = value;
		}
	}
}
