using System;
using System.Collections.Generic;
using ADOFAI.Common.Platform;
using UnityEngine;

public struct CalibrationPreset
{
	public const int DontModifyVisualOffset = -100000;

	public AudioOutputType outputType;

	public string outputName;

	public int inputOffset;

	public int priority;

	public bool confident;

	public Dictionary<string, object> ToDict()
	{
		return new Dictionary<string, object>
		{
			["outputType"] = outputType,
			["deviceName"] = outputName,
			["inputOffset"] = inputOffset
		};
	}

	public void FromDict(Dictionary<string, object> dict)
	{
		outputName = dict["deviceName"] as string;
		inputOffset = (int)dict["inputOffset"];
		outputType = RDUtils.ParseEnum(dict["outputType"] as string, AudioOutputType.Speaker);
		if (outputName == "*" && outputType == AudioOutputType.Speaker)
		{
			outputName = PlatformHelper.instance.GetActiveAudioDeviceName();
		}
	}

	public string ReadableOutputName()
	{
		if (outputType == AudioOutputType.Speaker)
		{
			if (!(outputName == "*"))
			{
				return outputName;
			}
			return RDString.Get("audioOutput.speaker");
		}
		if (outputType == AudioOutputType.Wired)
		{
			return RDString.Get("audioOutput.wired");
		}
		return outputName;
	}

	public static bool StringMatchesFilter(string s, string filter)
	{
		if (filter == "*")
		{
			return true;
		}
		if (filter.StartsWith("[[") && filter.EndsWith("]]"))
		{
			string value = filter.Substring(2, filter.Length - 4);
			return s.Contains(value);
		}
		return s == filter;
	}

	public static List<CalibrationPreset> LoadDefaults()
	{
		string[] array = Resources.Load<TextAsset>("CalibrationPresets").text.Split('\n', StringSplitOptions.None);
		bool flag = false;
		List<CalibrationPreset> list = new List<CalibrationPreset>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (flag)
			{
				string[] array3 = text.Split(',', StringSplitOptions.None);
				CalibrationPreset item = default(CalibrationPreset);
				string text2 = array3[0];
				if (text2 != "*" && ADOBase.platform != RDUtils.ParseEnum(text2, Platform.None))
				{
					continue;
				}
				string filter = array3[1];
				if (!StringMatchesFilter(SystemInfo.operatingSystem, filter))
				{
					continue;
				}
				string filter2 = array3[2];
				if (!StringMatchesFilter(SystemInfo.deviceModel, filter2))
				{
					continue;
				}
				string text3 = array3[3];
				item.outputType = ((text3 == "*") ? AudioOutputType.Any : RDUtils.ParseEnum(text3, AudioOutputType.Speaker));
				item.outputName = array3[4];
				item.inputOffset = int.Parse(array3[5]);
				item.priority = int.Parse(array3[6]);
				item.confident = array3[7].Contains("yes");
				list.Add(item);
			}
			if (text.StartsWith("Platform"))
			{
				flag = true;
			}
		}
		return list;
	}

	public override string ToString()
	{
		return $"type: {outputType}, name: {outputName}, inputOffset: {inputOffset}, priority: {priority}, confident: {confident}";
	}
}
