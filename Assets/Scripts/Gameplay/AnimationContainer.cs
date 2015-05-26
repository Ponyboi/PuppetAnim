using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("AnimationCollection")]
public class AnimationContainer
{
	[XmlArray("buffers"),XmlArrayItem("List<List<float>>")]
	public List<List<float>> buffers;
	public string path;

	public AnimationContainer() {
	}
	public AnimationContainer(List<List<float>> buffers, string path) {
		this.buffers = buffers;
		this.path = path;
	}
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(AnimationContainer));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static AnimationContainer Load(string path)
	{
		var serializer = new XmlSerializer(typeof(AnimationContainer));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as AnimationContainer;
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static AnimationContainer LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(AnimationContainer));
		return serializer.Deserialize(new StringReader(text)) as AnimationContainer;
	}
}