using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuPortalScreen : MobileMenuScreen
{
	public scrPortal portal;

	public string world;

	public int? stageForUnlock;

	public string[] unlockConditions;

	private bool crown;

	public override void Select(bool select, bool instant = false)
	{
		portal.FadePortal(select ? 1f : 0.2f, instant);
		portal.FadeCredits(select ? 1f : 0f, instant);
		portal.ShowStats(select, instant);
	}

	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_worldPortal).transform;
		portal = transform.GetComponent<scrPortal>();
		if (world == null)
		{
			return;
		}
		portal.world = world;
		portal.SetupCredits();
		portal.sprPortal.sprite = (GCS.FOOL_JOKER ? Resources.Load<Sprite>("InternalLevels/" + world + "/portal") : Resources.Load<Sprite>("PortalImages\\" + world));
		portal.usesCrownSign = crown;
		RectTransform statsCanvasMobilePlaceholder = portal.statsCanvasMobilePlaceholder;
		_ = portal.stats.transform;
		portal.statsText.gameObject.AddComponent<scrScaleByAspectRatio>().referenceAspectRatio = 1.7777778f;
		RectTransform statsTextContainer = portal.statsTextContainer;
		statsTextContainer.sizeDelta = statsCanvasMobilePlaceholder.sizeDelta;
		statsTextContainer.anchoredPosition = Vector2.zero;
		statsTextContainer.localPosition = statsCanvasMobilePlaceholder.localPosition;
		statsTextContainer.gameObject.SetActive(!ADOBase.isExpo);
		if (world.IsCrownWorld())
		{
			PortalSign component = Object.Instantiate(RDConstants.data.prefab_crownPortalSign, portal.transform).GetComponent<PortalSign>();
			component.transform.position = portal.sign.transform.position;
			portal.sign.gameObject.SetActive(value: false);
			portal.sign = component;
			ParticleSystem[] borderParticles = portal.borderParticles;
			foreach (ParticleSystem obj in borderParticles)
			{
				ParticleSystem.MainModule main = obj.main;
				main.startColor = "FFDE4E".HexToColor();
				obj.Simulate(2f);
				obj.Play();
			}
		}
		Select(select: false, instant: true);
	}

	public void CheckLocked(bool speedTrial)
	{
		int index = GCNS.worldData[portal.world].index;
		bool flag = Persistence.IsWorldComplete(portal.world);
		bool flag2 = Persistence.GetLevelTutorialProgress(index) > 0 || Persistence.GetWorldAttempts(index) > 0;
		bool locked = (speedTrial && !flag) || (!flag && !MobileMenuMap.EvaluateConditions(unlockConditions) && !flag2) || portal.medalLocked;
		portal.LockWorld(locked, speedTrial);
		portal.sign.UpdateWorldName(world, speedTrial);
	}

	public override void Decode(Dictionary<string, object> dict)
	{
		base.Decode(dict);
		world = (string)dict["world"];
		dict.TryGetValueAs("crown", out crown, _default: false);
		if (dict.TryGetValueAs<string, object, List<object>>("unlockedIf", out var valueAs))
		{
			unlockConditions = valueAs.OfType<string>().ToArray();
		}
	}

	public override string GetDescription()
	{
		string text = "";
		text = ((!portal.locked) ? (GCS.FOOL_JOKER ? RDString.Get(world + "-X.title") : RDString.Get("world" + world + ".description")) : RDString.Get("levelSelect.locked"));
		if (portal.world == "T4" && !Persistence.IsWorldComplete("T4"))
		{
			text = "???";
		}
		return text;
	}

	public override int GetDifficulty()
	{
		return GCNS.worldData[portal.world].difficulty;
	}

	public override float GetWidth()
	{
		return base.GetWidth() / 2f;
	}
}
