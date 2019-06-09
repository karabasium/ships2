using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LevelData {
	private int fieldWidth;
	private int fieldHeight;
	private List<Cell> cells;
	private List<Unit> units;

	public LevelData( string levelPath )
	{
		Cells = new List<Cell>();
		Units = new List<Unit>();

		TextAsset textAsset = (TextAsset)Resources.Load(levelPath);

		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(textAsset.text);

		XmlNodeList cellNodes = xmlDoc.GetElementsByTagName("cell");

		foreach (XmlNode node in cellNodes)
		{
			Debug.Log("type = " + node.Attributes["type"].Value);

			CellType cellType = CellType.SEA;
			if (node.Attributes["type"].Value == "land") {
				cellType = CellType.LAND;
			}
			else if (node.Attributes["type"].Value == "reefs")
			{
				cellType = CellType.REEFS;
			}
			else
			{
				Debug.Log("ERROR! Unknown type of cell = " + node.Attributes["type"].Value);
			}
			
			int x = System.Convert.ToUInt16( node.Attributes["x"].Value );
			int y = System.Convert.ToUInt16( node.Attributes["y"].Value );

			Cells.Add(new Cell( x, y, cellType ));
		}

		XmlNodeList unitNodes = xmlDoc.GetElementsByTagName("unit");

		foreach (XmlNode node in unitNodes)
		{
			Player player = Player.NONE;
			if (node.Attributes["player"].Value == "1")
			{
				player = Player.PLAYER_1;
			}
			else if (node.Attributes["player"].Value == "2")
			{
				player = Player.PLAYER_2;
			}
			else
			{
				Debug.Log("ERROR! Unknown player type = " + node.Attributes["player"].Value);
			}

			Unit u = new Unit(node.Attributes["class"].Value, player);

			int x = System.Convert.ToUInt16(node.Attributes["x"].Value);
			int y = System.Convert.ToUInt16(node.Attributes["y"].Value);

			u.Position = new Vector2Int(x, y);

			Units.Add(u);
		}

		XmlNodeList levelNodes = xmlDoc.GetElementsByTagName("level");
		Dictionary<string, string> levelParameters = new Dictionary<string, string>();
		FieldWidth = System.Convert.ToUInt16( levelNodes[0].Attributes["width"].Value);
		FieldHeight = System.Convert.ToUInt16(levelNodes[0].Attributes["height"].Value);
	}

	public int FieldWidth
	{
		get
		{
			return fieldWidth;
		}

		set
		{
			fieldWidth = value;
		}
	}

	public int FieldHeight
	{
		get
		{
			return fieldHeight;
		}

		set
		{
			fieldHeight = value;
		}
	}

	public List<Cell> Cells
	{
		get
		{
			return cells;
		}

		set
		{
			cells = value;
		}
	}

	public List<Unit> Units
	{
		get
		{
			return units;
		}

		set
		{
			units = value;
		}
	}
}
