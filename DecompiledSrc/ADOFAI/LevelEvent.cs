using System;
using System.Collections.Generic;
using ADOFAI.Editor.Models;
using GDMiniJSON;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ADOFAI;

public class LevelEvent
{
	public enum DecodeResult
	{
		Success,
		NoTypeFound,
		NeedsTaroDLC
	}

	public const int NoFloor = -1;

	public int floor = -1;

	public LevelEventType eventType;

	protected Dictionary<string, object> data;

	public Dictionary<string, bool> disabled;

	public bool isFake;

	public List<LevelEvent> realEvents = new List<LevelEvent>();

	public LevelEventInfo info;

	public bool active = true;

	public bool visible = true;

	public bool locked;

	public object this[string key]
	{
		get
		{
			return Get<object>(key);
		}
		set
		{
			data[key] = value;
		}
	}

	public bool IsDecoration => info.isDecoration;

	public T Get<T>(string key, T defaultValue = default(T))
	{
		if (data.TryGetValue(key, out var value))
		{
			return (T)value;
		}
		return defaultValue;
	}

	public bool TryGet<T>(string key, out T output)
	{
		if (data.TryGetValue(key, out var value))
		{
			output = (T)value;
			return true;
		}
		output = default(T);
		return false;
	}

	public bool TryGetAndSet<T>(string key, ref T output, bool onlyIfEnabled = false)
	{
		if (onlyIfEnabled && disabled[key])
		{
			return false;
		}
		if (data.TryGetValue(key, out var value))
		{
			output = (T)value;
			return true;
		}
		return false;
	}

	public Dictionary<string, object> GetData()
	{
		return data;
	}

	public bool ContainsKey(string key)
	{
		return data.ContainsKey(key);
	}

	public float GetFloat(string key)
	{
		return Get(key, 0f);
	}

	public string GetString(string key)
	{
		return Get<string>(key);
	}

	public int GetInt(string key)
	{
		return Get(key, 0);
	}

	public bool GetBool(string key)
	{
		return Get(key, defaultValue: false);
	}

	public Tuple<int, TileRelativeTo> GetTile(string key)
	{
		return Get<Tuple<int, TileRelativeTo>>(key);
	}

	public Tuple<float, float> GetFloatPair(string key)
	{
		return Get<Tuple<float, float>>(key);
	}

	public Color GetColor(string key)
	{
		return ((string)data[key]).HexToColor();
	}

	public ParticleSystem.MinMaxGradient GetMinMaxGradient(string key)
	{
		return Get<SerializedMinMaxGradient>(key).ToMinMaxGradient();
	}

	public T GetIfEnabled<T>(string key) where T : class
	{
		if (!disabled[key])
		{
			return Get<T>(key);
		}
		return null;
	}

	public Color? GetNullableColor(string key)
	{
		if (!disabled[key])
		{
			return GetColor(key);
		}
		return null;
	}

	public T? GetNullable<T>(string key) where T : struct
	{
		if (!disabled[key])
		{
			return Get<T>(key);
		}
		return null;
	}

	public string GetStringLocalized(string key)
	{
		string text = Get<string>(key);
		if (info.propertiesInfo[key].string_localizable)
		{
			int num = text.IndexOf("[[");
			int num2 = text.IndexOf("]]");
			if (num >= 0 && num2 > num)
			{
				bool exists;
				string withCheck = RDString.GetWithCheck(text.Substring(num + 2, num2 - num - 2), out exists);
				if (exists)
				{
					text = text.Substring(0, num) + withCheck + text.Substring(num2 + 2);
				}
			}
		}
		return text;
	}

	public LevelEvent(int newFloor, LevelEventType type)
		: this(newFloor, type, null, null, null, active: true, visible: true, locked: false)
	{
	}

	public LevelEvent(int newFloor, LevelEventType type, LevelEventInfo customInfo)
		: this(newFloor, type, customInfo, null, null, active: true, visible: true, locked: false)
	{
	}

	public LevelEvent(int newFloor, LevelEventType type, LevelEventInfo customInfo, Dictionary<string, object> data, Dictionary<string, bool> disabled, bool active, bool visible, bool locked)
	{
		floor = newFloor;
		eventType = type;
		this.visible = visible;
		this.locked = locked;
		this.active = active;
		string key = GCS.levelEventTypeString[eventType];
		info = ((customInfo != null) ? customInfo : GCS.levelEventsInfo[key]);
		if (data == null)
		{
			this.data = new Dictionary<string, object>();
			this.disabled = new Dictionary<string, bool>();
			{
				foreach (string key2 in info.propertiesInfo.Keys)
				{
					PropertyInfo propertyInfo = info.propertiesInfo[key2];
					this.data[key2] = propertyInfo.value_default;
					this.disabled[key2] = !propertyInfo.startEnabled;
				}
				return;
			}
		}
		this.data = new Dictionary<string, object>(data);
		this.disabled = new Dictionary<string, bool>(disabled);
	}

	public LevelEvent(Dictionary<string, object> dict)
	{
		Decode(dict);
	}

	public DecodeResult Decode(Dictionary<string, object> dict, string explicitEventType = null, bool isGlobal = false)
	{
		string text = ((explicitEventType != null) ? explicitEventType : (dict["eventType"] as string));
		eventType = RDUtils.ParseEnum(text, LevelEventType.None);
		if (eventType == LevelEventType.None)
		{
			return DecodeResult.NoTypeFound;
		}
		info = (isGlobal ? GCS.settingsInfo[text] : GCS.levelEventsInfo[text]);
		if (!info.taroDLCCheck)
		{
			return DecodeResult.NeedsTaroDLC;
		}
		if (dict.TryGetValue("floor", out var value))
		{
			floor = (int)value;
		}
		active = !dict.TryGetValue("active", out var value2) || !(value2 is bool flag) || flag;
		visible = !dict.TryGetValue("visible", out var value3) || !(value3 is bool flag2) || flag2;
		locked = dict.TryGetValue("locked", out var value4) && value4 is bool flag3 && flag3;
		FixDefaultValues(dict);
		data = new Dictionary<string, object>();
		disabled = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, PropertyInfo> item7 in info.propertiesInfo)
		{
			string key = item7.Key;
			PropertyInfo value5 = item7.Value;
			bool flag4 = Application.isEditor && !RDC.hideProEvents;
			if (value5.pro && !flag4 && !Persistence.enableProEvents && !ADOBase.isOfficialLevel)
			{
				if (value5.name != "components")
				{
					disabled.Add(key, value: true);
				}
				else
				{
					if (scnGame.instance.levelData.version > 12 || !dict.TryGetValue("components", out var value6))
					{
						continue;
					}
					string text2 = value6 as string;
					if (string.IsNullOrEmpty(text2) || !(Json.Deserialize("{" + text2.Trim() + "}") as Dictionary<string, object>).TryGetValue("scrLockToCamera", out var value7))
					{
						continue;
					}
					Dictionary<string, object> obj = value7 as Dictionary<string, object>;
					if (obj.TryGetValue("lockRot", out var value8) && value8 is bool flag5 && (!data.TryGetValue("scale", out var value9) || (value9 is Vector2 vector && Math.Abs(vector.x - vector.y) < 0.001f)))
					{
						data["lockRotation"] = flag5;
					}
					if (obj.TryGetValue("lockScale", out var value10) && value10 is bool flag6)
					{
						data["lockScale"] = flag6;
						if (flag6)
						{
							data["scaleMultiplier"] = scnGame.instance.levelData.camZoom / 240f;
						}
					}
					if (obj.TryGetValue("lockPos", out var value11) && value11 is bool flag7)
					{
						data["lockPos"] = flag7;
						if (flag7)
						{
							data["parallax"] = new Vector2(100f, 100f);
						}
					}
				}
			}
			else
			{
				if (key == "floor" || !value5.encode)
				{
					continue;
				}
				bool flag8 = !dict.ContainsKey(key);
				disabled.Add(key, flag8);
				if (flag8)
				{
					data.Add(key, value5.value_default);
					continue;
				}
				object obj2 = dict[key];
				if (value5.type == PropertyType.Enum)
				{
					if (obj2 is int)
					{
						data.Add(key, Enum.ToObject(value5.enumType, (int)obj2));
					}
					else
					{
						data.Add(key, Enum.Parse(value5.enumType, obj2 as string));
					}
				}
				else if (value5.type == PropertyType.Bool)
				{
					if (obj2 is string text3)
					{
						obj2 = text3 == "Enabled";
					}
					data.Add(key, obj2);
				}
				else if (value5.type == PropertyType.Float)
				{
					data.Add(key, Convert.ToSingle(obj2));
				}
				else if (value5.type == PropertyType.Int || value5.type == PropertyType.Rating)
				{
					data.Add(key, Convert.ToInt32(obj2));
				}
				else if (value5.type == PropertyType.Vector2)
				{
					if (obj2 is float num)
					{
						data.Add(key, new Vector2(num, num));
						continue;
					}
					if (obj2 is int num2)
					{
						data.Add(key, new Vector2(num2, num2));
						continue;
					}
					List<object> obj3 = obj2 as List<object>;
					float x = Convert.ToSingle(obj3[0] ?? ((object)float.NaN));
					float y = Convert.ToSingle(obj3[1] ?? ((object)float.NaN));
					data.Add(key, new Vector2(x, y));
				}
				else if (value5.type == PropertyType.Tile)
				{
					List<object> list = obj2 as List<object>;
					int item = Convert.ToInt32(list[0]);
					TileRelativeTo item2 = (TileRelativeTo)Enum.Parse(typeof(TileRelativeTo), list[1].ToString());
					data.Add(key, new Tuple<int, TileRelativeTo>(item, item2));
				}
				else if (value5.type == PropertyType.Array)
				{
					data.Add(key, RDEditorUtils.DecodeModsArray(obj2));
				}
				else if (value5.type == PropertyType.FilterProperties)
				{
					foreach (KeyValuePair<string, object> item8 in (obj2 is string) ? ((Dictionary<string, object>)Json.Deserialize("{" + obj2?.ToString() + "}")) : ((Dictionary<string, object>)obj2))
					{
						bool flag9 = false;
						object value12 = item8.Value;
						if (value12 is int || value12 is float)
						{
							data.Add(item8.Key, Convert.ToSingle(item8.Value));
						}
						else if (item8.Value is string)
						{
							data.Add(item8.Key, item8.Value);
						}
						else
						{
							if (item8.Value is List<object> { Count: 2 } list2)
							{
								value12 = list2[0];
								if (value12 is int || value12 is float)
								{
									value12 = list2[1];
									if (value12 is int || value12 is float)
									{
										data.Add(item8.Key, new Vector2(Convert.ToSingle(list2[0]), Convert.ToSingle(list2[1])));
										goto IL_077f;
									}
								}
							}
							flag9 = true;
						}
						goto IL_077f;
						IL_077f:
						if (!flag9)
						{
							disabled.Add(item8.Key, flag9);
						}
					}
					data.Add(key, null);
				}
				else if (value5.type == PropertyType.List)
				{
					data.Add(key, obj2);
				}
				else if (value5.type == PropertyType.FloatPair)
				{
					List<object> obj4 = obj2 as List<object>;
					float item3 = Convert.ToSingle(obj4[0] ?? ((object)float.NaN));
					float item4 = Convert.ToSingle(obj4[1] ?? ((object)float.NaN));
					data.Add(key, new Tuple<float, float>(item3, item4));
				}
				else if (value5.type == PropertyType.MinMaxGradient)
				{
					SerializedMinMaxGradient serializedMinMaxGradient = ((JToken)JObject.FromObject(obj2)).ToObject<SerializedMinMaxGradient>();
					data.Add(key, serializedMinMaxGradient);
				}
				else if (value5.type == PropertyType.Vector2Range)
				{
					List<object> obj5 = obj2 as List<object>;
					List<object> list3 = (List<object>)obj5[0];
					List<object> list4 = (List<object>)obj5[1];
					Vector2 item5 = new Vector2(Convert.ToSingle(list3[0] ?? ((object)float.NaN)), Convert.ToSingle(list3[1] ?? ((object)float.NaN)));
					Vector2 item6 = new Vector2(Convert.ToSingle(list4[0] ?? ((object)float.NaN)), Convert.ToSingle(list4[1] ?? ((object)float.NaN)));
					data.Add(key, new Tuple<Vector2, Vector2>(item5, item6));
				}
				else
				{
					data.Add(key, obj2);
				}
			}
		}
		return DecodeResult.Success;
	}

	private void FixDefaultValues(Dictionary<string, object> dict)
	{
		foreach (string item in new List<string>(info.propertiesInfo.Keys))
		{
			if (eventType == LevelEventType.AddDecoration && item == "decorationImage" && !dict.ContainsKey(item))
			{
				dict["decorationImage"] = dict["decText"];
				dict.Remove("decText");
			}
			if ((eventType == LevelEventType.AddDecoration || eventType == LevelEventType.AddText) && item == "parallax" && !dict.ContainsKey(item))
			{
				int num = (int)dict["depth"];
				num = ((num != 1 && num != -1) ? num : 0);
				dict["parallax"] = num;
			}
			if ((eventType == LevelEventType.CustomBackground || eventType == LevelEventType.BackgroundSettings) && item == "scalingRatio" && !dict.ContainsKey(item))
			{
				object value;
				bool flag = dict.TryGetValue("bgDisplayMode", out value) && RDUtils.ParseEnum(value as string, BgDisplayMode.FitToScreen) == BgDisplayMode.Unscaled;
				dict["scalingRatio"] = (flag ? ((int)dict["unscaledSize"]) : 100);
				dict.Remove("unscaledSize");
			}
			if (eventType == LevelEventType.AddDecoration && item == "hitbox" && dict.TryGetValue("failHitbox", out var value2))
			{
				bool flag2 = false;
				if (value2 is string text)
				{
					flag2 = text == "Enabled";
				}
				if (value2 is bool flag3)
				{
					flag2 = flag3;
				}
				dict.TryAdd(item, (object)(flag2 ? HitboxType.Kill : HitboxType.None).ToString());
			}
			if (eventType == LevelEventType.ScalePlanets && item == "targetPlanet" && dict.TryGetValue("targetPlanet", out var value3) && value3 is string text2 && text2 == "Both")
			{
				dict["targetPlanet"] = "All";
			}
		}
	}

	public Dictionary<string, object> Encode(bool settings = false)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!settings)
		{
			if (floor != -1)
			{
				dictionary.Add("floor", floor);
			}
			dictionary.Add("eventType", eventType.ToString());
			if (!active)
			{
				dictionary.Add("active", active);
			}
			if (!visible)
			{
				dictionary.Add("visible", visible);
			}
			if (locked)
			{
				dictionary.Add("locked", locked);
			}
		}
		foreach (string key in data.Keys)
		{
			_ = data[key];
			if (!info.propertiesInfo.ContainsKey(key) || (disabled.TryGetValue(key, out var value) && value && info.propertiesInfo[key].canBeDisabled))
			{
				continue;
			}
			PropertyInfo propertyInfo = info.propertiesInfo[key];
			if (propertyInfo.encode && !(key == "floor"))
			{
				PropertyType type = propertyInfo.type;
				switch (type)
				{
				case PropertyType.Bool:
				case PropertyType.Int:
				case PropertyType.String:
				case PropertyType.LongString:
				case PropertyType.Color:
				case PropertyType.File:
				case PropertyType.Rating:
				case PropertyType.Array:
				case PropertyType.MinMaxGradient:
					dictionary[key] = data[key];
					break;
				case PropertyType.Float:
					dictionary[key] = Convert.ToDecimal(data[key]);
					break;
				case PropertyType.Enum:
					dictionary[key] = data[key].ToString();
					break;
				case PropertyType.Vector2:
					dictionary[key] = RDEditorUtils.EncodeVector2((Vector2)data[key]);
					break;
				case PropertyType.Tile:
					dictionary[key] = RDEditorUtils.EncodeTile((Tuple<int, TileRelativeTo>)data[key]);
					break;
				case PropertyType.FilterProperties:
					dictionary[key] = RDEditorUtils.EncodeFilterProperties(data, disabled);
					break;
				case PropertyType.FloatPair:
					dictionary[key] = RDEditorUtils.EncodeFloatPair((Tuple<float, float>)data[key]);
					break;
				case PropertyType.Vector2Range:
					dictionary[key] = RDEditorUtils.EncodeVector2Range((Tuple<Vector2, Vector2>)data[key]);
					break;
				default:
					Debug.LogWarning(key + " not parsed! it is type: " + type);
					break;
				case PropertyType.Export:
					break;
				}
			}
		}
		return dictionary;
	}

	public static string EscapeTextForJSON(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		return text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\t")
			.Replace("\"", "\\\"");
	}

	public LevelEvent Copy()
	{
		return new LevelEvent(floor, eventType, info, data, disabled, active, visible, locked);
	}

	public LevelEvent CopyShallow()
	{
		return new LevelEvent(floor, eventType, info, data, disabled, active: true, visible: true, locked: false);
	}

	public override string ToString()
	{
		string text = "type: " + eventType.ToString() + "\n";
		foreach (KeyValuePair<string, object> datum in data)
		{
			text += $"{datum.Key}: {datum.Value}\n";
		}
		return text;
	}

	public void ApplyPropertiesToRealEvents()
	{
		foreach (string key in data.Keys)
		{
			if (disabled[key])
			{
				continue;
			}
			foreach (LevelEvent realEvent in realEvents)
			{
				PropertyInfo propertyInfo = info.propertiesInfo[key];
				if (propertyInfo.name == "floor")
				{
					realEvent.floor = floor;
				}
				else if (propertyInfo.type == PropertyType.Vector2)
				{
					Vector2 vector = (Vector2)realEvent.data[key];
					Vector2 vector2 = (Vector2)data[key];
					if (!float.IsNaN(vector2.x))
					{
						vector.x = vector2.x;
					}
					if (!float.IsNaN(vector2.y))
					{
						vector.y = vector2.y;
					}
					realEvent.data[key] = vector;
				}
				else
				{
					realEvent.data[key] = data[key];
				}
			}
		}
		if (!IsDecoration || !ADOBase.isEditingLevel)
		{
			return;
		}
		foreach (LevelEvent realEvent2 in realEvents)
		{
			ADOBase.editor.UpdateDecorationObject(realEvent2);
		}
	}
}
