using System;
using System.Collections.Generic;
using System.Reflection;
using ADOFAI;
using GDMiniJSON;

public class ffxAddComponent : ffxPlusBase
{
	public string componentName;

	public string properties;

	private bool isPlus;

	private ADOBase component;

	private Type componentType;

	public void Setup()
	{
		componentType = Type.GetType(componentName);
		if (floor == null)
		{
			floor = GetComponent<scrFloor>();
		}
		component = floor.gameObject.AddComponent(componentType) as ADOBase;
		isPlus = componentType.IsSubclassOf(typeof(ffxPlusBase));
		if (isPlus)
		{
			floor.plusEffects.Add((ffxPlusBase)component);
			((ffxPlusBase)component).duration = duration;
		}
		if (!(Json.Deserialize("{" + properties + "}") is Dictionary<string, object> dictionary))
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			FieldInfo field = componentType.GetField(item.Key);
			if (field != null)
			{
				field.SetValue(component, item.Value);
			}
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		base.SetStartTime(bpm, degreeOffset);
		if (component != null && isPlus)
		{
			(component as ffxPlusBase).SetStartTime(bpm, degreeOffset);
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		componentName = evnt.GetString("component");
		properties = evnt.GetString("properties");
		duration = evnt.GetFloat("duration") * crotchet;
		Setup();
	}
}
