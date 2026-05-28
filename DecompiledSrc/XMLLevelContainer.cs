using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("LevelCollection")]
public class XMLLevelContainer
{
	[XmlArray("Worlds")]
	[XmlArrayItem("World")]
	public List<XMLWorld> Worlds = new List<XMLWorld>();

	public void Save(string path)
	{
		Debug.Log("Test XML saving");
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLLevelContainer));
		using FileStream stream = new FileStream(path, FileMode.Create);
		xmlSerializer.Serialize(stream, this);
	}

	public static XMLLevelContainer Load(string path)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLLevelContainer));
		using FileStream stream = new FileStream(path, FileMode.Open);
		return xmlSerializer.Deserialize(stream) as XMLLevelContainer;
	}

	public static XMLLevelContainer LoadFromText(string text)
	{
		return new XmlSerializer(typeof(XMLLevelContainer)).Deserialize(new StringReader(text)) as XMLLevelContainer;
	}
}
