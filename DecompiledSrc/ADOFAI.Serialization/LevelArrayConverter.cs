using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace ADOFAI.Serialization;

public class LevelArrayConverter : JsonConverter<Dictionary<string, object>>
{
	private static string[] EventArrayKeys = new string[2] { "actions", "decorations" };

	public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, object> dict, JsonSerializerOptions options)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		writer.WriteStartObject();
		string text = default(string);
		object obj = default(object);
		foreach (KeyValuePair<string, object> item in dict)
		{
			item.Deconstruct(ref text, ref obj);
			string text2 = text;
			object obj2 = obj;
			writer.WritePropertyName(text2);
			if (obj2 is IList list)
			{
				JsonSerializerOptions val = new JsonSerializerOptions(options)
				{
					WriteIndented = false
				};
				if (EventArrayKeys.Contains(text2))
				{
					Debug.Log(text2);
					writer.WriteStartArray();
					foreach (object item2 in list)
					{
						string text3 = JsonSerializer.Serialize<object>(item2, val);
						writer.WriteRawValue("\n" + new string(' ', writer.CurrentDepth * 2) + text3, true);
					}
					writer.WriteEndArray();
					continue;
				}
				ArrayBufferWriter<byte> val2 = new ArrayBufferWriter<byte>();
				JsonWriterOptions val3 = default(JsonWriterOptions);
				((JsonWriterOptions)(ref val3)).Indented = false;
				Utf8JsonWriter val4 = new Utf8JsonWriter((IBufferWriter<byte>)(object)val2, val3);
				try
				{
					val4.WriteStartArray();
					foreach (object item3 in list)
					{
						JsonSerializer.Serialize<object>(val4, item3, val);
					}
					val4.WriteEndArray();
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
				writer.WriteRawValue(val2.WrittenSpan, true);
			}
			else
			{
				JsonSerializer.Serialize<object>(writer, obj2, (JsonSerializerOptions)null);
			}
		}
		writer.WriteEndObject();
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert == typeof(Dictionary<string, object>);
	}
}
