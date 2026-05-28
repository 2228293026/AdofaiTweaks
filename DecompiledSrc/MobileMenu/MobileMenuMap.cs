using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GDMiniJSON;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuMap
{
	public MobileMenuGroup rootGroup;

	public Dictionary<string, MobileMenuGroup> groupLUT;

	public Dictionary<string, MobileMenuPortalScreen> portalLUT;

	public Transform transform;

	public MobileMenuScreen mapCenter;

	private Transform pivotTransform;

	private const string MapsDirectory = "MobileMenuMaps";

	public MobileMenuMap(string mapName, bool build = false)
	{
		ImportMap(mapName, build);
	}

	public void ImportMap(string mapName, bool build = false)
	{
		Dictionary<string, object> dict = Json.Deserialize(Resources.Load<TextAsset>(Path.Combine("MobileMenuMaps", mapName)).text) as Dictionary<string, object>;
		Decode(dict);
		EvaluateAllConditions();
		if (build)
		{
			Build();
		}
	}

	public void SetMapCenter(MobileMenuScreen screen)
	{
		mapCenter = screen;
		pivotTransform.localPosition = pivotTransform.position - screen.transformPosition;
	}

	public void Decode(Dictionary<string, object> dict)
	{
		groupLUT = new Dictionary<string, MobileMenuGroup>();
		portalLUT = new Dictionary<string, MobileMenuPortalScreen>();
		foreach (object item in dict["groups"] as List<object>)
		{
			Dictionary<string, object> dict2 = item as Dictionary<string, object>;
			MobileMenuGroup mobileMenuGroup = new MobileMenuGroup();
			mobileMenuGroup.Decode(dict2);
			groupLUT.Add(mobileMenuGroup.id, mobileMenuGroup);
			foreach (MobileMenuScreen screen in mobileMenuGroup.screens)
			{
				if (screen is MobileMenuPortalScreen)
				{
					MobileMenuPortalScreen mobileMenuPortalScreen = screen as MobileMenuPortalScreen;
					portalLUT.Add(mobileMenuPortalScreen.world, mobileMenuPortalScreen);
				}
			}
		}
		string key = (string)dict["rootGroup"];
		rootGroup = groupLUT[key];
	}

	public void Build(bool instantiate = false)
	{
		if (transform == null)
		{
			transform = new GameObject("Mobile Menu Map Container").transform;
		}
		if (pivotTransform == null)
		{
			pivotTransform = new GameObject("Pivot").transform;
			pivotTransform.SetParent(transform);
		}
		BuildGroupRecursively(rootGroup);
		if (instantiate)
		{
			InstantiateScreenTransforms();
		}
	}

	private MobileMenuGroup BuildGroupRecursively(MobileMenuGroup group, MoveDirection spawnDirection = MoveDirection.Up, Vector2 position = default(Vector2))
	{
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int vector = spawnDirection.GetVector();
		bool flag = spawnDirection == MoveDirection.Left;
		position.y += group.GetHeight() / 2f * (float)vector.y;
		bool flag2 = false;
		do
		{
			group.visibleScreens = new List<MobileMenuScreen>();
			position.x += group.horizontalGap * MobileMenuScreen.GetBaseWidth(group.zoom) * (float)spawnDirection.GetVector().x;
			for (int i = 0; i < group.screens.Count; i++)
			{
				MobileMenuScreen mobileMenuScreen = group.screens[i];
				if (mobileMenuScreen.visible)
				{
					flag2 = true;
					group.visibleScreens.Add(mobileMenuScreen);
					mobileMenuScreen.parentGroup = group;
					float width = mobileMenuScreen.GetWidth();
					float num = ((MobileMenuScreen.GetAspect() < 1.5f) ? width : (width / 2f));
					position.x += num * (float)spawnDirection.GetVector().x;
					mobileMenuScreen.RepositionTransform(position, pivotTransform);
					spawnDirection = (flag ? MoveDirection.Left : MoveDirection.Right);
					position.x += mobileMenuScreen.GetWidth() / 2f * (float)spawnDirection.GetVector().x;
				}
			}
			if (!flag2)
			{
				if (!group.groupToSpawn.ContainsKey(spawnDirection))
				{
					return null;
				}
				group = groupLUT[group.groupToSpawn[spawnDirection]];
			}
		}
		while (!flag2);
		group.linkedGroup = new Dictionary<MoveDirection, MobileMenuGroup>();
		foreach (KeyValuePair<MoveDirection, string> item in group.groupToSpawn)
		{
			MoveDirection key = item.Key;
			string value = item.Value;
			Vector2Int vector2 = key.GetVector();
			List<MobileMenuScreen> visibleScreens = group.visibleScreens;
			Index val = (Index)((key == MoveDirection.Right) ? new Index(1, true) : Index.op_Implicit(0));
			MobileMenuScreen mobileMenuScreen2 = visibleScreens[((Index)(ref val)).GetOffset(visibleScreens.Count)];
			Vector3 transformPosition = mobileMenuScreen2.transformPosition;
			transformPosition.x += mobileMenuScreen2.GetWidth() / 2f * (float)vector2.x;
			transformPosition.y += group.GetHeight() / 2f * (float)vector2.y;
			MobileMenuGroup mobileMenuGroup = groupLUT[value];
			MobileMenuGroup mobileMenuGroup2 = BuildGroupRecursively(mobileMenuGroup, key, transformPosition);
			if (mobileMenuGroup2 != null)
			{
				mobileMenuGroup2.linkedGroup.Add(key.Invert(), group);
				group.linkedGroup.Add(key, mobileMenuGroup2);
			}
		}
		return group;
	}

	public IEnumerator InstantiateScreenTransformsCo()
	{
		foreach (MobileMenuGroup value in groupLUT.Values)
		{
			foreach (MobileMenuScreen screen in value.screens)
			{
				if (screen.visible)
				{
					if (!screen.transform)
					{
						screen.Instantiate();
					}
					screen.RepositionTransform();
					yield return new WaitForEndOfFrame();
				}
			}
		}
	}

	public void InstantiateScreenTransforms()
	{
		foreach (MobileMenuGroup value in groupLUT.Values)
		{
			foreach (MobileMenuScreen screen in value.screens)
			{
				if (screen.visible)
				{
					if (!screen.transform)
					{
						screen.Instantiate();
					}
					screen.RepositionTransform();
				}
			}
		}
	}

	public void EvaluateAllConditions()
	{
		foreach (MobileMenuGroup value in groupLUT.Values)
		{
			foreach (MobileMenuScreen screen in value.screens)
			{
				bool visible = EvaluateConditions(screen.visibilityConditions);
				screen.visible = visible;
			}
		}
	}

	public static bool EvaluateConditions(string[] conditions)
	{
		bool flag = true;
		if (conditions != null)
		{
			for (int i = 0; i < conditions.Length; i++)
			{
				string[] array = conditions[i].Split(':', StringSplitOptions.None);
				string text = array[0];
				string text2 = ((array.Length > 1) ? array[1] : "");
				switch (text)
				{
				case "completed":
					flag &= Persistence.IsWorldComplete(GCNS.worldData[text2].index);
					break;
				case "speedTrial":
					flag &= Persistence.IsSpeedTrialComplete(GCNS.worldData[text2].index);
					break;
				case "stage":
					flag &= Persistence.GetOverallProgressStage() >= int.Parse(text2);
					break;
				case "condition":
					flag &= CheckCustomCondition(text2);
					break;
				case "singleplayer":
					flag &= scrPlayerManager.playerCount == 1;
					break;
				case "moreIsVisible":
					flag &= ADOBase.isSwitch;
					break;
				case "showTech":
					flag &= Persistence.ShowTechLevels();
					break;
				}
			}
		}
		return flag;
	}

	private static bool CheckCustomCondition(string world)
	{
		switch (world)
		{
		case "XF":
			return Persistence.unlockedXF;
		case "XC":
			return Persistence.unlockedXC;
		case "XR":
			return Persistence.unlockedXR;
		case "XH":
			return Persistence.unlockedXH;
		case "TaroRift":
			if (!Persistence.IsWorldComplete("T1EX") && !Persistence.IsWorldComplete("T2EX") && !Persistence.IsWorldComplete("T3EX"))
			{
				return Persistence.IsWorldComplete("T4EX");
			}
			return true;
		case "NeoCosmosAd":
			if (Persistence.GetOverallProgressStage() >= 1)
			{
				return !NeoCosmosManager.instance.installed;
			}
			return false;
		default:
			return false;
		}
	}
}
