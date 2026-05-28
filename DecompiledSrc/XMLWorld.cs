using System.Collections.Generic;
using System.Xml.Serialization;

public class XMLWorld
{
	[XmlArray("Levels")]
	[XmlArrayItem("Level")]
	public List<XMLLevel> Levels = new List<XMLLevel>();
}
