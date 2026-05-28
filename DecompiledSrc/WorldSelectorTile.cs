using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldSelectorTile : ffxPlusBase
{
	public enum WorldClearType
	{
		NormalClear,
		SpeedTrialClear
	}

	[FormerlySerializedAs("worldIndex")]
	public string world;

	public bool speedTrial;

	[Header("Components")]
	public Sprite levelIcon;

	public LevelSelectIsland island;

	public scrBestMultiplierText bestText;

	[Header("Unlock requirement")]
	public string worldToRequireClearing;

	public WorldClearType worldClearType;

	public float speedTrialCompletionRequirement;

	private new void Awake()
	{
		base.Awake();
		if (speedTrial)
		{
			island.speedTrialTiles.TryAdd(world, this);
		}
		else
		{
			island.worldTiles.TryAdd(world, this);
		}
		if (speedTrial && !Persistence.IsWorldComplete(ADOBase.worldData[world].index))
		{
			base.enabled = false;
			return;
		}
		if (worldToRequireClearing != "")
		{
			int index = ADOBase.worldData[worldToRequireClearing].index;
			if (worldClearType == WorldClearType.NormalClear)
			{
				if (!Persistence.IsWorldComplete(index))
				{
					base.enabled = false;
					return;
				}
			}
			else if (Mathf.Max(Persistence.GetBestSpeedMultiplier(index, coop: false), Persistence.GetBestSpeedMultiplier(index, coop: true)) < speedTrialCompletionRequirement)
			{
				base.enabled = false;
				return;
			}
		}
		StartCoroutine(SetIcon());
	}

	private void OnEnable()
	{
		StartCoroutine(SetIcon());
	}

	public override void StartEffect(scrPlanet planet)
	{
		island.SelectWorld(world, speedTrial);
	}

	private IEnumerator SetIcon()
	{
		yield return null;
		floor.SetIconSprite(levelIcon);
		if (speedTrial)
		{
			floor.SetIconColor(Color.red);
		}
	}
}
