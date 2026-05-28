using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class testdropdowninstantiator : ADOBase
{
	public GameObject dropdownPrefab;

	public int itemCount;

	public bool includeFilterItems;

	public bool includeEaseItems;

	[Header("Total dropdown count")]
	[Tooltip("Only up to 4 is allowed.")]
	public int dropdownCount;

	public InputField itemValueInputField;

	private List<TweakableDropdown> tweakableDropdowns;

	private static Array filters = Enum.GetValues(typeof(Filter));

	private static List<Ease> eases = Enum.GetValues(typeof(Ease)).Cast<Ease>().ToList();

	private static readonly string[] constantItemValues = new string[13]
	{
		"heop", "mmmmmmmmmm beep beep beep", "ccccccccccccccc", "I ate expired pie", "this is a great dropdown", "i dont really know what to write here", "string", "restrict judgment", "'use strict';", "I use microsoft edge for my IDE",
		"hi have you ever checked your game's file integrit y befoerg upladogns issuesssf", "MonsterLove State Machine", "why use string when you have char[]"
	};

	[Header("Custom Items")]
	private List<string> customItems;

	private const string randomCharset = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890 ";

	private void Start()
	{
		tweakableDropdowns = new List<TweakableDropdown>();
		float? num = null;
		eases.Remove(Ease.INTERNAL_Custom);
		eases.Remove(Ease.INTERNAL_Zero);
		eases.Remove(Ease.Unset);
		switch (dropdownCount)
		{
		case 1:
			SpawnDropdown();
			break;
		case 2:
			SpawnDropdown(createPos(-50f, 0f));
			SpawnDropdown(createPos(50f, 0f));
			num = 200f;
			break;
		case 3:
			SpawnDropdown(createPos(-50f, 80f));
			SpawnDropdown(createPos(50f, 80f));
			SpawnDropdown(createPos(0f, -40f));
			num = 200f;
			break;
		case 4:
			SpawnDropdown(createPos(-80f, -80f));
			SpawnDropdown(createPos(-80f, 80f));
			SpawnDropdown(createPos(80f, -80f));
			SpawnDropdown(createPos(80f, 80f));
			num = 200f;
			break;
		}
		if (!num.HasValue)
		{
			return;
		}
		float valueOrDefault = num.GetValueOrDefault();
		foreach (TweakableDropdown tweakableDropdown in tweakableDropdowns)
		{
			tweakableDropdown.itemWidth = valueOrDefault;
			Vector2 sizeDelta = ((RectTransform)tweakableDropdown.gameObject.transform).sizeDelta;
			sizeDelta.x = valueOrDefault;
			((RectTransform)tweakableDropdown.gameObject.transform).sizeDelta = sizeDelta;
			tweakableDropdown.Setup();
			tweakableDropdown.ReloadList();
		}
		static Vector3 createPos(float x, float y)
		{
			return new Vector3(x * 2.35f, y * 2.35f);
		}
	}

	private TweakableDropdown SpawnDropdown(Vector3? positionDifference = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(dropdownPrefab);
		TweakableDropdown component = gameObject.GetComponent<TweakableDropdown>();
		if (!includeFilterItems)
		{
			filters = new object[0];
		}
		if (!includeEaseItems)
		{
			eases = new List<Ease>();
		}
		RectTransform rectTransform = (RectTransform)gameObject.transform;
		rectTransform.SetParent(base.transform);
		gameObject.SetActive(value: true);
		dropdownPrefab.SetActive(value: false);
		rectTransform.localPosition = new Vector3(-154.17f, 29.76f);
		if (positionDifference.HasValue)
		{
			Vector3 valueOrDefault = positionDifference.GetValueOrDefault();
			rectTransform.localPosition += valueOrDefault;
		}
		string[] array = constantItemValues;
		foreach (string item in array)
		{
			component.itemValues.Add(item);
		}
		foreach (Filter filter in filters)
		{
			component.itemValues.Add(RDString.Get($"enum.Filter.{filter}"));
		}
		foreach (Ease ease in eases)
		{
			component.itemValues.Add(RDString.Get($"enum.Ease.{ease}"));
		}
		for (int j = 0; j < itemCount - (constantItemValues.Length + filters.Length); j++)
		{
			component.itemValues.Add(GetRandomString());
		}
		component.ReloadList();
		tweakableDropdowns.Add(component);
		return component;
	}

	private string GetRandomString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = UnityEngine.Random.Range(8, 30);
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890 "[UnityEngine.Random.Range(0, "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890 ".Length - 1)]);
		}
		return stringBuilder.ToString();
	}

	public void AddItem()
	{
		foreach (TweakableDropdown tweakableDropdown in tweakableDropdowns)
		{
			tweakableDropdown.AddItem(itemValueInputField.text);
		}
	}

	public void RemoveItem()
	{
		foreach (TweakableDropdown tweakableDropdown in tweakableDropdowns)
		{
			tweakableDropdown.RemoveItem(itemValueInputField.text);
		}
	}
}
