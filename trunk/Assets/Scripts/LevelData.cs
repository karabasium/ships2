using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LevelData {
	private int fieldWidth;
	private int fieldHeight;
	private List<Cell> cells;
	private List<Unit> units;
	private string path;
	public List<LevelInfo> landAppearancesInfo = new List<LevelInfo>();

	public struct LevelInfo
	{
		public string Name;
		public Vector2 Position;
		public LevelInfo( string name, Vector2 position)
		{
			Name = name;
			Position = position;
		}
	}

	public LevelData( string levelPath )
	{
		Cells = new List<Cell>();
		Units = new List<Unit>();
		path = "Assets/Resources/" + levelPath + ".xml";

		TextAsset textAsset = (TextAsset)Resources.Load(levelPath);

		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(textAsset.text);

		XmlNodeList cellNodes = xmlDoc.GetElementsByTagName("cell");

		foreach (XmlNode node in cellNodes)
		{
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

		XmlNodeList landAppearances = xmlDoc.GetElementsByTagName("landAppearance");
		foreach (XmlNode node in landAppearances)
		{
			string objName = node.Attributes["prefab_name"].Value;
			//GameObject g = (GameObject)Resources.Load("Prefabs/" + objName);
			Debug.Log(objName);
			Vector2 pos = new Vector2(System.Convert.ToSingle(node.Attributes["world_x"].Value, System.Globalization.CultureInfo.InvariantCulture), System.Convert.ToSingle(node.Attributes["world_y"].Value, System.Globalization.CultureInfo.InvariantCulture));
			LevelInfo levelInfo = new LevelInfo(objName, pos);
			landAppearancesInfo.Add(levelInfo);
			Debug.Log("Land Object info loaded");
		}

		XmlNodeList levelNodes = xmlDoc.GetElementsByTagName("level");
		FieldWidth = System.Convert.ToUInt16( levelNodes[0].Attributes["width"].Value);
		FieldHeight = System.Convert.ToUInt16(levelNodes[0].Attributes["height"].Value);
	}

	public void Save()
	{
		XmlDocument xmlDoc = new XmlDocument();
		Field f = GameController.instance.f;

		XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
		XmlElement root = xmlDoc.DocumentElement;
		xmlDoc.InsertBefore(xmlDeclaration, root);

		XmlElement levelElement = xmlDoc.CreateElement("level");
		xmlDoc.AppendChild(levelElement);
		levelElement.SetAttribute("width", f.Width.ToString());
		levelElement.SetAttribute("height", f.Height.ToString());

		XmlElement cellNodes = xmlDoc.CreateElement("cells");
		levelElement.AppendChild(cellNodes);
		foreach (Cell cell in f.GetAllCells())
		{
			if (cell.CellType != CellType.SEA)
			{
				XmlElement cellNode = xmlDoc.CreateElement("cell");
				cellNodes.AppendChild(cellNode);
				cellNode.SetAttribute("x", cell.X.ToString());
				cellNode.SetAttribute("y", cell.Y.ToString());

				if (cell.CellType == CellType.LAND)
				{
					cellNode.SetAttribute("type", "land");
				}
				else if (cell.CellType == CellType.REEFS)
				{
					cellNode.SetAttribute("type", "reefs");
				}
			}
		}

		XmlElement unitNodes = xmlDoc.CreateElement("units");
		levelElement.AppendChild(unitNodes);
		foreach (Unit unit in f.GetUnits())
		{
			XmlElement unitNode = xmlDoc.CreateElement("unit");
			unitNodes.AppendChild(unitNode);
			unitNode.SetAttribute("x", unit.Position.x.ToString());
			unitNode.SetAttribute("y", unit.Position.y.ToString());
			unitNode.SetAttribute("class", unit.Unit_class);
			if (unit.Player == Player.PLAYER_1)
			{
				unitNode.SetAttribute("player", "1");
			}
			else if (unit.Player == Player.PLAYER_2)
			{
				unitNode.SetAttribute("player", "2");
			}
		}

		XmlElement landAppearanceNodes = xmlDoc.CreateElement("landAppearances");
		levelElement.AppendChild(landAppearanceNodes);
		foreach (GameObject g in GameController.instance.fa.LandObjects)
		{
			XmlElement landAppearanceNode = xmlDoc.CreateElement("landAppearance");
			landAppearanceNodes.AppendChild(landAppearanceNode);
			Vector2 undistortedPos = Utils.TransformToUndistorted(g.transform.position);
			landAppearanceNode.SetAttribute("world_x", undistortedPos.x.ToString());
			landAppearanceNode.SetAttribute("world_y", undistortedPos.y.ToString());
			landAppearanceNode.SetAttribute("prefab_name", g.name);
		}

		xmlDoc.Save( path );
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
