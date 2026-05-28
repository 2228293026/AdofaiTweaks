using System.Collections.Generic;
using UnityEngine;

public class LevelSelectIsland : ADOBase
{
	public GCNS.WorldData.IslandType islandType;

	public scrFloor portalFloor;

	public scrFloor speedTrialToggle;

	public scrFloor speedTrialToggleOther;

	public bool speedTrial;

	private string selectedWorld;

	public readonly Dictionary<string, WorldSelectorTile> worldTiles = new Dictionary<string, WorldSelectorTile>();

	public readonly Dictionary<string, WorldSelectorTile> speedTrialTiles = new Dictionary<string, WorldSelectorTile>();

	private void Awake()
	{
		if (!portalFloor.dontChangeMySprite)
		{
			portalFloor.dontChangeMySprite = true;
			portalFloor.topGlow.color = Color.clear;
			if ((bool)portalFloor.bottomGlow)
			{
				portalFloor.bottomGlow.color = Color.clear;
			}
		}
	}

	private void Start()
	{
		ADOBase.levelSelect.islands.Add(islandType, this);
		SetSpeedTrial(speedTrial, flash: false);
	}

	public void DeselectWorld()
	{
		selectedWorld = null;
		UpdateSpeedTrial(null);
		if (portalFloor.isportal)
		{
			portalFloor.isportal = false;
			portalFloor.CheckPortalSprite();
			portalFloor.arguments = "";
			portalFloor.GetComponentInChildren<scrPortalParticles>().disabled = true;
			if (selectedWorld != null)
			{
				scrPortal.portals[selectedWorld].ExpandPortal(expand: false, instant: true);
			}
		}
	}

	public void SelectWorld(string world = null, bool speedTrial = false)
	{
		selectedWorld = world;
		if (scnLevelSelect.instance != null)
		{
			scnLevelSelect.instance.lastVisitedWorld = world;
		}
		foreach (LevelSelectIsland value in ADOBase.levelSelect.islands.Values)
		{
			if (value != this)
			{
				value.DeselectWorld();
			}
		}
		if (world == null)
		{
			DeselectWorld();
			return;
		}
		if (!(speedTrial ? speedTrialTiles : worldTiles)[world].enabled)
		{
			DeselectWorld();
			return;
		}
		scrFlash.Flash(Color.white.WithAlpha(0.4f));
		portalFloor.isportal = true;
		portalFloor.CheckPortalSprite();
		portalFloor.levelnumber = (speedTrial ? Portal.GoToLevelSpeedTrial : Portal.GoToWorldBossIfReached);
		portalFloor.arguments = (speedTrial ? (world + "-X") : world);
		scrPortalParticles componentInChildren = portalFloor.GetComponentInChildren<scrPortalParticles>();
		componentInChildren.speedTrial = speedTrial;
		componentInChildren.disabled = false;
		foreach (scrPortal value2 in scrPortal.portals.Values)
		{
			if (!value2.world.IsXtra() && !value2.world.IsMuseDashWorld())
			{
				continue;
			}
			if (value2.world != world)
			{
				value2.ExpandPortal(expand: false, instant: true);
				continue;
			}
			value2.ExpandPortal(expand: true, instant: true);
			if (!(value2.xtraDecoration != null))
			{
				continue;
			}
			SpriteRenderer xtraDecoration = value2.xtraDecoration;
			if (xtraDecoration != null && !base.gameObject.name.Contains("DONT"))
			{
				xtraDecoration.color = (speedTrial ? new Color(1f, 0.7f, 0.7f) : Color.white);
			}
			SpriteRenderer[] componentsInChildren = value2.xtraDecoration.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				if (!spriteRenderer.gameObject.name.Contains("DONT"))
				{
					spriteRenderer.color = (speedTrial ? new Color(1f, 0.7f, 0.7f) : Color.white);
				}
			}
		}
		UpdateSpeedTrial(world, speedTrial);
	}

	public void UpdateSpeedTrial(string world, bool speedTrial = false)
	{
		foreach (WorldSelectorTile value in speedTrialTiles.Values)
		{
			scrBestMultiplierText bestText = value.bestText;
			if (!(bestText == null))
			{
				if (speedTrialToggle == null)
				{
					bestText.gameObject.SetActive(speedTrial && value.world == world);
					bestText.UpdateText(showLongVersion: true);
				}
				else
				{
					bestText.gameObject.SetActive(value.enabled);
					bestText.UpdateText(value.world == world);
				}
			}
		}
	}

	public void ToggleSpeedTrial()
	{
		SetSpeedTrial(!speedTrial);
		if (scnLevelSelect.instance != null)
		{
			scnLevelSelect.instance.DeferReunifyPlanets();
		}
	}

	public void SetSpeedTrial(bool speedTrial, bool flash = true)
	{
		if (speedTrialToggle == null)
		{
			return;
		}
		this.speedTrial = speedTrial;
		scrFlash.Flash((speedTrial ? Color.red : Color.blue).WithAlpha(0.4f));
		SelectWorld(selectedWorld, speedTrial);
		speedTrialToggle?.gameObject.SetActive(!speedTrial);
		foreach (WorldSelectorTile value in worldTiles.Values)
		{
			value.gameObject.SetActive(!speedTrial);
		}
		speedTrialToggleOther?.gameObject.SetActive(speedTrial);
		foreach (WorldSelectorTile value2 in speedTrialTiles.Values)
		{
			value2.gameObject.SetActive(speedTrial);
		}
	}
}
