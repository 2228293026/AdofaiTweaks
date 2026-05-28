using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxSetFilterAdvancedPlus : ffxPlusBase
{
	public static readonly string[] blacklistedFilterKeywords = new string[3] { "Blend2Camera_", "Antialiasing_FXAA", "Colors_Adjust_PreFilters" };

	public FilterTargetType targetType;

	public FilterPlane plane;

	public string decorationTag;

	public string filterName;

	public bool enableFilter;

	public bool disableOthers;

	public Dictionary<string, object> filterProperties;

	private List<GameObject> targetObjects;

	private static Dictionary<FilterPlane, Camera> planeToCamera = new Dictionary<FilterPlane, Camera>();

	private static Dictionary<GameObject, HashSet<string>> addedFilters = new Dictionary<GameObject, HashSet<string>>();

	private static Dictionary<GameObject, HashSet<string>> usedFilters = new Dictionary<GameObject, HashSet<string>>();

	private static Dictionary<GameObject, HashSet<string>> modifiedFilters = new Dictionary<GameObject, HashSet<string>>();

	private static Dictionary<GameObject, Dictionary<string, Dictionary<string, object>>> filterOriginalValues = new Dictionary<GameObject, Dictionary<string, Dictionary<string, object>>>();

	private static Dictionary<GameObject, Dictionary<string, Dictionary<string, Tween>>> filterFieldTweens = new Dictionary<GameObject, Dictionary<string, Dictionary<string, Tween>>>();

	private static Dictionary<GameObject, HashSet<string>> initializedFilters = new Dictionary<GameObject, HashSet<string>>();

	private Type filterType;

	private FieldInfo[] filterFields;

	private Dictionary<GameObject, Component> filterComponents = new Dictionary<GameObject, Component>();

	private List<GameObject> isAddedComponent = new List<GameObject>();

	private Dictionary<GameObject, MonoBehaviour> filterMonoBehaviours = new Dictionary<GameObject, MonoBehaviour>();

	private bool saveOriginalValue;

	protected override IEnumerable<Tween> eventTweens => from d in filterFieldTweens.Values.SelectMany((Dictionary<string, Dictionary<string, Tween>> d) => (!d.TryGetValue(filterName, out var value)) ? Enumerable.Empty<Tween>() : value.Values)
		where d != null
		select d;

	private static bool variableSetupDone => CollectionExtensions.GetValueOrDefault<FilterPlane, Camera>((IReadOnlyDictionary<FilterPlane, Camera>)planeToCamera, FilterPlane.Foreground) != null;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public static void ResetVariables()
	{
		addedFilters.Clear();
	}

	private static void SetupVariables()
	{
		if (variableSetupDone)
		{
			return;
		}
		foreach (FilterPlane value2 in Enum.GetValues(typeof(FilterPlane)))
		{
			if (!(CollectionExtensions.GetValueOrDefault<FilterPlane, Camera>((IReadOnlyDictionary<FilterPlane, Camera>)planeToCamera, value2) != null))
			{
				Camera value = null;
				switch (value2)
				{
				case FilterPlane.Foreground:
					value = scrCamera.instance.camobj;
					break;
				case FilterPlane.Background:
					value = scrCamera.instance.BGcam;
					break;
				}
				planeToCamera[value2] = value;
			}
		}
	}

	public void Setup()
	{
		SetupVariables();
		targetObjects = targetType switch
		{
			FilterTargetType.Camera => new List<GameObject> { planeToCamera[plane].gameObject }, 
			FilterTargetType.Decoration => (from d in scrDecorationManager.instance.GetTaggedDecorations(decorationTag)
				select d.gameObject).ToList(), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		filterType = Type.GetType(filterName + ", Assembly-CSharp-firstpass");
		filterFields = filterType.GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (GameObject targetObject in targetObjects)
		{
			addedFilters.TryAdd(targetObject, new HashSet<string>());
			usedFilters.TryAdd(targetObject, new HashSet<string>());
			modifiedFilters.TryAdd(targetObject, new HashSet<string>());
			filterOriginalValues.TryAdd(targetObject, new Dictionary<string, Dictionary<string, object>>());
			filterFieldTweens.TryAdd(targetObject, new Dictionary<string, Dictionary<string, Tween>>());
			initializedFilters.TryAdd(targetObject, new HashSet<string>());
			if (targetObject.TryGetComponent(filterType, out var component))
			{
				filterComponents[targetObject] = component;
				saveOriginalValue = !addedFilters[targetObject].Contains(filterName) && !usedFilters[targetObject].Contains(filterName) && !modifiedFilters[targetObject].Contains(filterName);
				if (saveOriginalValue)
				{
					filterOriginalValues[targetObject][filterName] = new Dictionary<string, object>();
				}
				filterMonoBehaviours[targetObject] = (MonoBehaviour)component;
				modifiedFilters[targetObject].Add(filterName);
			}
			else
			{
				Component component2 = (filterComponents[targetObject] = targetObject.AddComponent(filterType));
				component = component2;
				isAddedComponent.Add(targetObject);
				MonoBehaviour monoBehaviour = (filterMonoBehaviours[targetObject] = (MonoBehaviour)component);
				monoBehaviour.enabled = false;
				addedFilters[targetObject].Add(filterName);
			}
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (blacklistedFilterKeywords.Any(filterName.Contains))
		{
			return;
		}
		foreach (GameObject targetObject in targetObjects)
		{
			Component filterComponent = filterComponents[targetObject];
			MonoBehaviour monoBehaviour = filterMonoBehaviours[targetObject];
			if (monoBehaviour == null)
			{
				Debug.LogError($"Filter component not found, floor: {floor.seqID}, level: {ADOBase.levelPath}");
				continue;
			}
			if (disableOthers)
			{
				ResetFilters(targetObject, resetAll: false);
			}
			bool flag = initializedFilters[targetObject].Contains(filterName);
			FieldInfo[] array = filterFields;
			foreach (FieldInfo field in array)
			{
				if (!enableFilter)
				{
					if (filterFieldTweens[targetObject].TryGetValue(filterName, out var value) && value.TryGetValue(field.Name, out var value2))
					{
						value2.Kill(complete: true);
					}
				}
				else
				{
					if (monoBehaviour == null)
					{
						continue;
					}
					bool flag2 = monoBehaviour.enabled;
					Type fieldType = field.FieldType;
					bool flag3 = fieldType == typeof(int);
					bool flag4 = fieldType == typeof(float);
					bool flag5 = flag3 || flag4;
					bool flag6 = fieldType == typeof(Color);
					bool flag7 = fieldType == typeof(Vector2);
					if (!flag5 && !flag6 && !flag7)
					{
						continue;
					}
					float num = RDEditorUtils.UnitMultiplier(RDEditorUtils.FilterFieldToUnit(RDEditorUtils.TypeToPropertyType(fieldType), field.Name));
					object value3;
					bool flag8 = !filterProperties.TryGetValue(field.Name, out value3);
					if (flag4)
					{
						float num2 = (float)field.GetValue(filterComponent);
						if (saveOriginalValue)
						{
							filterOriginalValues[targetObject][filterName][field.Name] = num2;
						}
						float num3 = ((!flag8) ? ((float)value3 / num) : (flag ? num2 : 0f));
						if (duration == 0f)
						{
							field.SetValue(filterComponent, num3);
							continue;
						}
						if (!filterFieldTweens[targetObject].TryGetValue(filterName, out var value4))
						{
							Dictionary<string, Tween> dictionary = (filterFieldTweens[targetObject][filterName] = new Dictionary<string, Tween>());
							value4 = dictionary;
						}
						float currValue = ((flag && flag2) ? num2 : 0f);
						field.SetValue(filterComponent, currValue);
						if (value4.TryGetValue(field.Name, out var value5))
						{
							value5.Kill(complete: true);
						}
						value4[field.Name] = DOTween.To(() => currValue, delegate(float num6)
						{
							currValue = num6;
							field.SetValue(filterComponent, num6);
						}, num3, duration).SetEase(ease);
					}
					else if (flag3)
					{
						int num4 = (int)field.GetValue(filterComponent);
						if (saveOriginalValue)
						{
							filterOriginalValues[targetObject][filterName][field.Name] = num4;
						}
						int num5 = ((!flag8) ? ((int)Convert.ToSingle(Convert.ToSingle(value3) / num)) : (flag ? num4 : 0));
						if (duration == 0f)
						{
							field.SetValue(filterComponent, num5);
							continue;
						}
						if (!filterFieldTweens[targetObject].TryGetValue(filterName, out var value6))
						{
							Dictionary<string, Tween> dictionary = (filterFieldTweens[targetObject][filterName] = new Dictionary<string, Tween>());
							value6 = dictionary;
						}
						int currValue2 = ((flag && flag2) ? num4 : 0);
						field.SetValue(filterComponent, currValue2);
						if (value6.TryGetValue(field.Name, out var value7))
						{
							value7.Kill(complete: true);
						}
						value6[field.Name] = DOTween.To(() => currValue2, delegate(int num6)
						{
							currValue2 = num6;
							field.SetValue(filterComponent, num6);
						}, num5, duration).SetEase(ease);
					}
					else if (flag6)
					{
						Color color = (Color)field.GetValue(filterComponent);
						if (saveOriginalValue)
						{
							filterOriginalValues[targetObject][filterName][field.Name] = color;
						}
						Color color2 = ((!flag8) ? ((string)value3).HexToColor() : (flag ? color : Color.white));
						if (duration == 0f)
						{
							field.SetValue(filterComponent, color2);
							continue;
						}
						if (!filterFieldTweens[targetObject].TryGetValue(filterName, out var value8))
						{
							Dictionary<string, Tween> dictionary = (filterFieldTweens[targetObject][filterName] = new Dictionary<string, Tween>());
							value8 = dictionary;
						}
						Color currValue3 = ((flag && flag2) ? color : Color.white);
						field.SetValue(filterComponent, currValue3);
						if (value8.TryGetValue(field.Name, out var value9))
						{
							value9.Kill(complete: true);
						}
						value8[field.Name] = DOTween.To(() => currValue3, delegate(Color color3)
						{
							currValue3 = color3;
							field.SetValue(filterComponent, color3);
						}, color2, duration).SetEase(ease);
					}
					else
					{
						if (!flag7)
						{
							continue;
						}
						Vector2 vector = (Vector2)field.GetValue(filterComponent);
						if (saveOriginalValue)
						{
							filterOriginalValues[targetObject][filterName][field.Name] = vector;
						}
						Vector2 vector2 = ((!flag8) ? ((Vector2)value3) : (flag ? vector : Vector2.zero));
						if (duration == 0f)
						{
							field.SetValue(filterComponent, vector2);
							continue;
						}
						if (!filterFieldTweens[targetObject].TryGetValue(filterName, out var value10))
						{
							Dictionary<string, Tween> dictionary = (filterFieldTweens[targetObject][filterName] = new Dictionary<string, Tween>());
							value10 = dictionary;
						}
						Vector2 currValue4 = ((flag && flag2) ? vector : Vector2.zero);
						field.SetValue(filterComponent, currValue4);
						if (value10.TryGetValue(field.Name, out var value11))
						{
							value11.Kill(complete: true);
						}
						value10[field.Name] = DOTween.To(() => currValue4, delegate(Vector2 vector3)
						{
							currValue4 = vector3;
							field.SetValue(filterComponent, vector3);
						}, vector2, duration).SetEase(ease);
					}
				}
			}
			monoBehaviour.enabled = enableFilter;
			usedFilters[targetObject].Add(filterName);
			initializedFilters[targetObject].Add(filterName);
		}
	}

	private void OnDestroy()
	{
		foreach (GameObject targetObject in targetObjects)
		{
			if (targetObject == null)
			{
				continue;
			}
			if (isAddedComponent.Contains(targetObject))
			{
				UnityEngine.Object.DestroyImmediate(filterComponents[targetObject]);
				addedFilters[targetObject].Remove(filterName);
			}
			if (!initializedFilters[targetObject].Contains(filterName))
			{
				continue;
			}
			usedFilters[targetObject].Remove(filterName);
			if (filterFieldTweens[targetObject].TryGetValue(filterName, out var value))
			{
				foreach (Tween value2 in value.Values)
				{
					value2.Kill(complete: true);
				}
			}
			filterFieldTweens[targetObject].Remove(filterName);
			initializedFilters[targetObject].Remove(filterName);
		}
	}

	public static void CleanVariables(GameObject targetObj)
	{
		addedFilters.Remove(targetObj);
		usedFilters.Remove(targetObj);
		modifiedFilters.Remove(targetObj);
		filterOriginalValues.Remove(targetObj);
		filterFieldTweens.Remove(targetObj);
		initializedFilters.Remove(targetObj);
	}

	public static void ResetFilterValues(Component filterComponent, Type filterType = null)
	{
		if (!variableSetupDone)
		{
			return;
		}
		if ((object)filterType == null)
		{
			filterType = filterComponent.GetType();
		}
		if (!filterOriginalValues[filterComponent.gameObject].TryGetValue(filterType.Name, out var value))
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in value)
		{
			filterType.GetField(item.Key).SetValue(filterComponent, item.Value);
		}
	}

	public static void ResetAllFilters()
	{
		foreach (GameObject key in initializedFilters.Keys)
		{
			if (key != null)
			{
				ResetFilters(key);
			}
		}
	}

	public static void ResetFilters(GameObject targetObj, bool resetAll = true)
	{
		foreach (string item in usedFilters[targetObj])
		{
			Type type = Type.GetType(item + ", Assembly-CSharp-firstpass");
			Component component = targetObj.GetComponent(type);
			if (component == null)
			{
				continue;
			}
			((MonoBehaviour)component).enabled = false;
			if (!resetAll)
			{
				ResetFilterValues(component, type);
			}
			if (!filterFieldTweens[targetObj].TryGetValue(item, out var value))
			{
				continue;
			}
			foreach (Tween value2 in value.Values)
			{
				value2.Kill(complete: true);
			}
		}
		usedFilters[targetObj].Clear();
		filterFieldTweens[targetObj].Clear();
		if (resetAll)
		{
			initializedFilters[targetObj].Clear();
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
		targetType = FilterTargetType.Camera;
		plane = (FilterPlane)evnt["plane"];
		decorationTag = evnt.GetString("targetTag");
		filterName = evnt.GetString("filter");
		enableFilter = evnt.GetBool("enabled");
		disableOthers = evnt.GetBool("disableOthers");
		filterProperties = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> datum in evnt.GetData())
		{
			if (datum.Key.StartsWith("filter_") && !evnt.disabled[datum.Key])
			{
				object value = datum.Value;
				if (value is int || value is float || value is string || value is Vector2)
				{
					filterProperties.Add(datum.Key.Replace("filter_", ""), datum.Value);
				}
			}
		}
		Setup();
	}
}
