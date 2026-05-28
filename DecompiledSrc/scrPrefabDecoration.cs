using System;
using UnityEngine;

public class scrPrefabDecoration : scrDecoration
{
	public PublicPrefabType prefabType;

	public override string gameObjectName => sourceLevelEvent.GetString("decorationImage")?.RemoveRichTags();

	public override string decorationName => "<color=#9D00FF>" + gameObjectName.Replace("prefab:", "", StringComparison.OrdinalIgnoreCase) + "</color>";

	public override void HitFloor()
	{
	}

	public override void SetDepth(int depth)
	{
	}

	protected override void ApplyColor()
	{
	}

	public override void SetVisible(bool visible)
	{
	}

	public void SetTile(Vector2 newTile)
	{
	}

	public override float GetAlpha()
	{
		return 1f;
	}
}
