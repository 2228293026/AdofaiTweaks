using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuGroup
{
	[Serializable]
	public struct BackgroundTheme
	{
		public Color? backgroundColor;

		public Color? gradientColor;

		public Color? cloudColor;

		public int? backgroundId;

		public BackgroundTheme WithDefaults(BackgroundTheme defaultTheme)
		{
			Color? color = backgroundColor;
			if (!color.HasValue)
			{
				backgroundColor = defaultTheme.backgroundColor;
			}
			color = gradientColor;
			if (!color.HasValue)
			{
				gradientColor = defaultTheme.gradientColor;
			}
			color = cloudColor;
			if (!color.HasValue)
			{
				cloudColor = defaultTheme.cloudColor;
			}
			int? num = backgroundId;
			if (!num.HasValue)
			{
				backgroundId = defaultTheme.backgroundId;
			}
			return this;
		}

		public BackgroundTheme Decode(Dictionary<string, object> dict)
		{
			if (dict.TryGetValueAs<string, object, string>("backgroundColor", out var valueAs))
			{
				backgroundColor = valueAs.HexToColor();
			}
			if (dict.TryGetValueAs<string, object, string>("gradientColor", out var valueAs2))
			{
				gradientColor = valueAs2.HexToColor();
			}
			if (dict.TryGetValueAs<string, object, string>("cloudColor", out var valueAs3))
			{
				cloudColor = valueAs3.HexToColor();
			}
			if (dict.TryGetValueAs("backgroundID", out var valueAs4, 0))
			{
				backgroundId = valueAs4;
			}
			return this;
		}
	}

	public string id;

	public List<MobileMenuScreen> screens;

	public List<MobileMenuScreen> visibleScreens;

	public string captionKey;

	public float horizontalGap;

	public float height = 1f;

	public float zoom;

	public bool inaccessible;

	public Dictionary<MoveDirection, MobileMenuGroup> linkedGroup;

	public Dictionary<MoveDirection, string> groupToSpawn;

	public BackgroundTheme theme;

	public BackgroundTheme speedTheme;

	public MobileMenuScreen this[int index] => visibleScreens[index];

	public IEnumerator<MobileMenuScreen> GetEnumerator()
	{
		return visibleScreens.GetEnumerator();
	}

	public void Decode(Dictionary<string, object> dict)
	{
		id = dict["name"] as string;
		screens = new List<MobileMenuScreen>();
		groupToSpawn = new Dictionary<MoveDirection, string>();
		foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection)))
		{
			if (dict.TryGetValueAs<string, object, string>(value.ToString().ToLower() + "Group", out var valueAs))
			{
				groupToSpawn.Add(value, valueAs);
			}
		}
		dict.TryGetValueAs("horizontalGap", out horizontalGap, 0f);
		dict.TryGetValueAs<string, object, string>("caption", out captionKey);
		dict.TryGetValueAs("zoom", out zoom, 4f);
		if (dict.TryGetValueAs<string, object, Dictionary<string, object>>("theme", out var valueAs2))
		{
			theme = theme.Decode(valueAs2);
		}
		if (dict.TryGetValueAs<string, object, Dictionary<string, object>>("speedTheme", out var valueAs3))
		{
			speedTheme = speedTheme.Decode(valueAs3);
		}
		foreach (object item in dict["screens"] as List<object>)
		{
			Dictionary<string, object> dictionary = item as Dictionary<string, object>;
			MobileMenuScreen mobileMenuScreen = MobileMenuScreen.New((string)dictionary["type"]);
			if (mobileMenuScreen != null)
			{
				mobileMenuScreen.Decode(dictionary);
				screens.Add(mobileMenuScreen);
			}
		}
	}

	public float GetHeight()
	{
		return height * 2.5f * Camera.main.orthographicSize;
	}
}
