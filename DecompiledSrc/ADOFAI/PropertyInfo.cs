using System;
using System.Collections.Generic;
using ADOFAI.Editor.Models;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ADOFAI;

public class PropertyInfo
{
	public string name;

	public int order;

	public PropertyType type;

	public MonoBehaviour control;

	public ControlType controlType;

	public Type enumType;

	public object[] enumExceptions;

	public string enumTypeString;

	public string unit;

	public string placeholderKey;

	public string placeholder;

	public string customLocalizationKey;

	public string customLabel;

	public bool pro;

	public Dictionary<string, object> dict;

	public LevelEventInfo levelEventInfo;

	public bool canBeDisabled;

	public bool startEnabled = true;

	public bool stringDropdown;

	public bool invisible;

	public bool slider;

	public bool ignoreRange;

	public string groupName;

	public List<Tuple<string, string>> disableIfVals = new List<Tuple<string, string>>();

	public List<Tuple<string, string>> enableIfVals = new List<Tuple<string, string>>();

	public List<Tuple<string, string>> hideIfVals = new List<Tuple<string, string>>();

	public List<Tuple<string, string>> showIfVals = new List<Tuple<string, string>>();

	public bool isEnabled = true;

	public bool hasRandomValue;

	public string randModeKey;

	public string randValueKey;

	public object value_default;

	public int int_min;

	public int int_max;

	public float float_min;

	public float float_max;

	public bool vector2_allowEmpty;

	public bool color_usesAlpha;

	public int string_minLength;

	public int string_maxLength;

	public bool string_needsUnicode;

	public bool string_localizable;

	public FileType fileType;

	public float floatPairMin;

	public float floatPairMax;

	public float floatPairStep;

	public bool floatPairIsRange;

	public string noteKey;

	public Array enumValidation;

	public Vector2 minVec;

	public Vector2 maxVec;

	public bool required;

	public bool browsable;

	public bool encode;

	public bool affectsFloors;

	public bool affectsPath;

	private bool isFloor;

	public PropertyInfo(Dictionary<string, object> dict, LevelEventInfo levelEventInfo)
	{
		this.dict = dict;
		this.levelEventInfo = levelEventInfo;
		name = dict["name"] as string;
		groupName = (string)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "group", (object)null);
		string text = dict["type"] as string;
		object obj = null;
		bool flag = false;
		if (dict.ContainsKey("default"))
		{
			obj = dict["default"];
			flag = true;
		}
		if (name == "floor")
		{
			isFloor = true;
		}
		if (dict.ContainsKey("unit"))
		{
			unit = dict["unit"] as string;
		}
		if (dict.ContainsKey("key"))
		{
			customLocalizationKey = dict["key"] as string;
		}
		if (dict.ContainsKey("customLabel"))
		{
			customLabel = dict["customLabel"] as string;
		}
		if (dict.ContainsKey("pro"))
		{
			pro = (bool)dict["pro"];
		}
		if (dict.ContainsKey("canBeDisabled"))
		{
			canBeDisabled = (bool)dict["canBeDisabled"];
		}
		if (dict.ContainsKey("placeholder"))
		{
			placeholderKey = dict["placeholder"] as string;
		}
		if (dict.TryGetValue("stringDropdown", out var value))
		{
			stringDropdown = (bool)value;
		}
		if (dict.TryGetValue("invisible", out value))
		{
			invisible = (bool)value;
		}
		if (dict.TryGetValue("slider", out value))
		{
			slider = (bool)value;
		}
		if (dict.TryGetValue("ignoreRange", out value))
		{
			ignoreRange = (bool)value;
		}
		if (canBeDisabled)
		{
			startEnabled = false;
			if (dict.ContainsKey("startEnabled"))
			{
				startEnabled = (bool)dict["startEnabled"];
			}
		}
		dict.ContainsKey("hasRandomValue");
		if (dict.ContainsKey("enableIf"))
		{
			string[] array = RDEditorUtils.DecodeStringArray(dict["enableIf"]);
			if (array.Length % 2 != 0)
			{
				Debug.Log("Not all keys have values");
			}
			else
			{
				for (int i = 0; i < array.Length; i += 2)
				{
					string item = array[i];
					string item2 = array[i + 1];
					enableIfVals.Add(new Tuple<string, string>(item, item2));
				}
			}
		}
		if (dict.ContainsKey("disableIf"))
		{
			string[] array2 = RDEditorUtils.DecodeStringArray(dict["disableIf"]);
			if (array2.Length % 2 != 0)
			{
				Debug.Log("Not all keys have values");
			}
			else
			{
				for (int j = 0; j < array2.Length; j += 2)
				{
					string item3 = array2[j];
					string item4 = array2[j + 1];
					disableIfVals.Add(new Tuple<string, string>(item3, item4));
				}
			}
		}
		if (dict.ContainsKey("showIf"))
		{
			string[] array3 = RDEditorUtils.DecodeStringArray(dict["showIf"]);
			if (array3.Length % 2 != 0)
			{
				Debug.Log("Not all keys have values");
			}
			else
			{
				for (int k = 0; k < array3.Length; k += 2)
				{
					string item5 = array3[k];
					string item6 = array3[k + 1];
					showIfVals.Add(new Tuple<string, string>(item5, item6));
				}
			}
		}
		if (dict.ContainsKey("hideIf"))
		{
			string[] array4 = RDEditorUtils.DecodeStringArray(dict["hideIf"]);
			if (array4.Length % 2 != 0)
			{
				Debug.Log("Not all keys have values");
			}
			else
			{
				for (int l = 0; l < array4.Length; l += 2)
				{
					string item7 = array4[l];
					string item8 = array4[l + 1];
					hideIfVals.Add(new Tuple<string, string>(item7, item8));
				}
			}
		}
		if (dict.ContainsKey("required"))
		{
			required = (bool)dict["required"];
		}
		if (dict.ContainsKey("browsable"))
		{
			browsable = (bool)dict["browsable"];
			fileType = (dict["fileType"] as string).ToEnum(FileType.Audio);
		}
		encode = !dict.ContainsKey("encode") || (bool)dict["encode"];
		affectsFloors = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "affectsFloors", (object)false);
		affectsPath = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "affectsPath", (object)false);
		switch (text)
		{
		case "Float":
			type = PropertyType.Float;
			value_default = (flag ? Convert.ToSingle(obj) : 0f);
			float_min = (dict.ContainsKey("min") ? Convert.ToSingle(dict["min"]) : float.NegativeInfinity);
			float_max = (dict.ContainsKey("max") ? Convert.ToSingle(dict["max"]) : float.PositiveInfinity);
			controlType = ControlType.InputField;
			break;
		case "Bool":
			type = PropertyType.Bool;
			value_default = flag && (bool)obj;
			controlType = ControlType.ToggleGroup;
			break;
		case "Int":
			type = PropertyType.Int;
			value_default = (flag ? Convert.ToInt32(obj) : 0);
			int_min = (dict.ContainsKey("min") ? Convert.ToInt32(dict["min"]) : int.MinValue);
			int_max = (dict.ContainsKey("max") ? Convert.ToInt32(dict["max"]) : int.MaxValue);
			controlType = ControlType.InputField;
			break;
		case "Color":
		{
			type = PropertyType.Color;
			color_usesAlpha = !dict.ContainsKey("usesAlpha") || Convert.ToBoolean(dict["usesAlpha"]);
			string text2 = (color_usesAlpha ? "ffffffff" : "ffffff");
			value_default = (flag ? (obj as string) : text2);
			controlType = ControlType.ColorPicker;
			break;
		}
		case "File":
			type = PropertyType.File;
			value_default = (flag ? (obj as string) : string.Empty);
			fileType = (dict["fileType"] as string).ToEnum(FileType.Audio);
			controlType = ControlType.File;
			break;
		case "String":
			type = PropertyType.String;
			if (stringDropdown)
			{
				value_default = (flag ? (obj as string) : string.Empty);
			}
			else
			{
				value_default = (flag ? RDString.Get(obj as string) : string.Empty);
			}
			string_minLength = (dict.ContainsKey("minLength") ? Convert.ToInt32(dict["minLength"]) : int.MinValue);
			string_maxLength = (dict.ContainsKey("maxLength") ? Convert.ToInt32(dict["maxLength"]) : int.MaxValue);
			string_needsUnicode = !dict.ContainsKey("needsUnicode") || Convert.ToBoolean(dict["needsUnicode"]);
			string_localizable = dict.ContainsKey("localizable") && Convert.ToBoolean(dict["localizable"]);
			controlType = ControlType.InputField;
			break;
		case "Text":
			type = PropertyType.LongString;
			value_default = (flag ? RDString.Get(obj as string) : string.Empty);
			controlType = ControlType.LongInputField;
			break;
		case "Vector2":
			type = PropertyType.Vector2;
			vector2_allowEmpty = dict.ContainsKey("allowEmpty") && Convert.ToBoolean(dict["allowEmpty"]);
			if (flag)
			{
				List<object> obj2 = obj as List<object>;
				float x = Convert.ToSingle(obj2[0] ?? ((object)float.NaN));
				float y = Convert.ToSingle(obj2[1] ?? ((object)float.NaN));
				value_default = new Vector2(x, y);
			}
			else
			{
				value_default = new Vector2(0f, 0f);
			}
			if (dict.ContainsKey("min"))
			{
				List<object> obj3 = dict["min"] as List<object>;
				float x = Convert.ToSingle(obj3[0]);
				float y = Convert.ToSingle(obj3[1]);
				minVec = new Vector2(x, y);
			}
			else
			{
				minVec = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
			}
			if (dict.ContainsKey("max"))
			{
				List<object> obj4 = dict["max"] as List<object>;
				float x = Convert.ToSingle(obj4[0]);
				float y = Convert.ToSingle(obj4[1]);
				maxVec = new Vector2(x, y);
			}
			else
			{
				maxVec = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			}
			break;
		case "Vector2Range":
			type = PropertyType.Vector2Range;
			floatPairIsRange = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "isRange", (object)true);
			if (flag)
			{
				List<object> obj5 = (List<object>)obj;
				List<object> list2 = (List<object>)obj5[0];
				List<object> list3 = (List<object>)obj5[1];
				value_default = new Tuple<Vector2, Vector2>(new Vector2(Convert.ToSingle(list2[0]), Convert.ToSingle(list2[1])), new Vector2(Convert.ToSingle(list3[0]), Convert.ToSingle(list3[1])));
			}
			break;
		case "Tile":
			type = PropertyType.Tile;
			enumType = typeof(TileRelativeTo);
			if (flag)
			{
				List<object> list = obj as List<object>;
				int item9 = Convert.ToInt32(list[0]);
				TileRelativeTo item10 = (TileRelativeTo)Enum.Parse(typeof(TileRelativeTo), Convert.ToString(list[1]));
				value_default = new Tuple<int, TileRelativeTo>(item9, item10);
			}
			else
			{
				value_default = new Tuple<int, TileRelativeTo>(0, TileRelativeTo.ThisTile);
			}
			int_min = (dict.ContainsKey("min") ? Convert.ToInt32(dict["min"]) : int.MinValue);
			int_max = (dict.ContainsKey("max") ? Convert.ToInt32(dict["max"]) : int.MaxValue);
			break;
		case "FloatPair":
		{
			type = PropertyType.FloatPair;
			floatPairMin = (floatPairMax = -2.1474836E+09f);
			floatPairMax = 2.1474836E+09f;
			floatPairStep = 0.1f;
			floatPairIsRange = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "isRange", (object)true);
			if (dict.TryGetValue("min", out var value2))
			{
				floatPairMin = Convert.ToSingle(value2);
			}
			if (dict.TryGetValue("max", out var value3))
			{
				floatPairMax = Convert.ToSingle(value3);
			}
			if (dict.TryGetValue("step", out var value4))
			{
				floatPairStep = Convert.ToSingle(value4);
			}
			if (flag)
			{
				List<object> list4 = obj as List<object>;
				value_default = new Tuple<float, float>(Convert.ToSingle(list4[0] ?? ((object)float.NaN)), Convert.ToSingle(list4[1] ?? ((object)float.NaN)));
			}
			controlType = ControlType.FloatPair;
			break;
		}
		case "MinMaxGradient":
			type = PropertyType.MinMaxGradient;
			value_default = (flag ? ((JToken)JObject.FromObject(obj)).ToObject<SerializedMinMaxGradient>() : SerializedMinMaxGradient.Default());
			controlType = ControlType.MinMaxGradient;
			break;
		case "Export":
			type = PropertyType.Export;
			break;
		case "Rating":
			type = PropertyType.Rating;
			value_default = (flag ? Convert.ToInt32(obj) : 0);
			break;
		case "Array":
			type = PropertyType.Array;
			value_default = (flag ? obj : new object[0]);
			break;
		case "List":
			type = PropertyType.List;
			break;
		case "FilterProperties":
			type = PropertyType.FilterProperties;
			break;
		case "ParticlePlayback":
			type = PropertyType.ParticlePlayback;
			controlType = ControlType.ParticlePlayback;
			break;
		case "Note":
			type = PropertyType.Note;
			noteKey = (string)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)dict, "noteKey", (object)("editor." + levelEventInfo.name + "." + name + ".note"));
			break;
		default:
			if (text.StartsWith("Enum:"))
			{
				type = PropertyType.Enum;
				enumTypeString = text.Replace("Enum:", string.Empty);
				if (enumTypeString == "Ease")
				{
					enumType = typeof(Ease);
				}
				else if (enumTypeString == "ParticleArcMode")
				{
					enumType = typeof(ParticleSystemShapeMultiModeValue);
				}
				else
				{
					enumType = Type.GetType(enumTypeString);
				}
				value_default = (flag ? Enum.Parse(enumType, obj as string) : ((object)0));
				if (!flag)
				{
					Debug.LogWarning("Default value for type: " + enumType?.ToString() + " doesn't exist; it should for enums.");
				}
				controlType = ControlType.Dropdown;
			}
			else
			{
				Debug.LogWarning("didn't recognize type: " + text);
			}
			break;
		}
		if (enumType != null && dict.TryGetValue("except", out var value5))
		{
			List<object> list5 = (List<object>)value5;
			enumExceptions = new object[list5.Count];
			int num = 0;
			foreach (string item11 in list5)
			{
				enumExceptions[num] = Enum.Parse(enumType, item11);
				num++;
			}
		}
		if (dict.ContainsKey("control"))
		{
			controlType = RDUtils.ParseEnum(dict["control"] as string, ControlType.NotAssigned);
		}
		dict.TryGetValueAs<string, object, string>("placeholder", out placeholder);
	}

	public float Validate(float value)
	{
		float value2 = value;
		if (!ignoreRange)
		{
			return Mathf.Clamp(value2, float_min, float_max);
		}
		return value;
	}

	public int Validate(int value)
	{
		int min = ((!isFloor) ? int_min : 0);
		int max = (isFloor ? (scnGame.instance.levelMaker.listFloors.Count - 1) : int_max);
		if (!ignoreRange)
		{
			return Mathf.Clamp(value, min, max);
		}
		return value;
	}

	public Vector2 Validate(Vector2 value, bool forceAllowEmpty = false)
	{
		Vector2 vector = (Vector2)value_default;
		float num = value.x;
		float num2 = value.y;
		if (!vector2_allowEmpty && !forceAllowEmpty)
		{
			num = (float.IsNaN(num) ? vector.x : num);
			num2 = (float.IsNaN(num2) ? vector.y : num2);
		}
		float x = Mathf.Clamp(num, minVec.x, maxVec.x);
		float y = Mathf.Clamp(num2, minVec.y, maxVec.y);
		return new Vector2(x, y);
	}

	public Tuple<float, float> Validate(Tuple<float, float> value)
	{
		Tuple<float, float> tuple = (Tuple<float, float>)value_default;
		value.Deconstruct(out var item, out var item2);
		float num = item;
		float num2 = item2;
		num = (float.IsNaN(num) ? tuple.Item1 : num);
		num2 = (float.IsNaN(num2) ? tuple.Item2 : num2);
		float item3 = Mathf.Clamp(num, floatPairMin, floatPairMax);
		float item4 = Mathf.Clamp(num2, floatPairMin, floatPairMax);
		return new Tuple<float, float>(item3, item4);
	}

	public bool CheckIfEnabled(LevelEvent currentEvent, string group)
	{
		if (!CheckIfShown(currentEvent, group))
		{
			return false;
		}
		bool flag = true;
		bool flag2 = true;
		if (enableIfVals.Count != 0)
		{
			flag = ValueMatch(enableIfVals, currentEvent);
		}
		if (disableIfVals.Count != 0)
		{
			flag2 = !ValueMatch(disableIfVals, currentEvent);
		}
		return flag && flag2;
	}

	public bool CheckIfShown(LevelEvent currentEvent, string group)
	{
		if (invisible)
		{
			return false;
		}
		if (levelEventInfo.useGroups && groupName != group)
		{
			return false;
		}
		bool flag = true;
		bool flag2 = true;
		if (showIfVals.Count != 0)
		{
			flag = ValueMatch(showIfVals, currentEvent);
		}
		if (hideIfVals.Count != 0)
		{
			flag2 = !ValueMatch(hideIfVals, currentEvent);
		}
		return flag && flag2;
	}

	private bool ValueMatch(List<Tuple<string, string>> keysAndVals, LevelEvent currentEvent)
	{
		foreach (Tuple<string, string> keysAndVal in keysAndVals)
		{
			if (currentEvent == null)
			{
				return false;
			}
			PropertyInfo propertyInfo = levelEventInfo.propertiesInfo[keysAndVal.Item1];
			string item = keysAndVal.Item2;
			object obj = currentEvent[propertyInfo.name];
			switch (propertyInfo.type)
			{
			case PropertyType.Int:
			case PropertyType.Rating:
				if (Convert.ToInt32(item) == (int)obj)
				{
					return true;
				}
				break;
			case PropertyType.Float:
				if (Mathf.Approximately(Convert.ToSingle(item), (float)obj))
				{
					return true;
				}
				break;
			case PropertyType.String:
			case PropertyType.File:
			case PropertyType.Enum:
			case PropertyType.List:
				if (obj == null)
				{
					return false;
				}
				if (item == obj.ToString())
				{
					return true;
				}
				break;
			case PropertyType.Vector2:
			{
				if (currentEvent.disabled[propertyInfo.name])
				{
					return false;
				}
				Vector2 vector = (Vector2)obj;
				if (item == $"{vector.x},{vector.y}")
				{
					return true;
				}
				break;
			}
			case PropertyType.Bool:
				if ((item == "true" || item == "Enabled") == (bool)obj)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}
}
