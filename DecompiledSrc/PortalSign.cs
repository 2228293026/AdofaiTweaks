using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PortalSign : ADOBase
{
	public SpriteRenderer sign;

	public Text worldName;

	public GameObject padlockContainer;

	public SpriteRenderer padlock1;

	public SpriteRenderer padlock2;

	public WorldLightsDisplay lanterns;

	private void Awake()
	{
		worldName.SetLocalizedFont();
	}

	public void Fade(float alpha, float duration)
	{
		worldName.DOKill();
		sign.DOKill();
		worldName.DOFade(alpha, duration);
		sign.DOFade(alpha, duration);
		lanterns.Fade(alpha, duration);
		if (padlock1 != null)
		{
			padlock1.DOKill();
			padlock1.DOFade(alpha, duration);
		}
		if (padlock2 != null)
		{
			padlock2.DOKill();
			padlock2.DOFade(alpha, duration);
		}
	}

	public void UpdateWorldName(string world, bool speedTrial = false)
	{
		int num;
		if (GCS.FOOL_JOKER)
		{
			num = (world.EndsWith("J") ? 1 : 0);
			if (num != 0)
			{
				world = world.Substring(0, world.Length - 1);
			}
		}
		else
		{
			num = 0;
		}
		string text = RDString.Get("levelSelect.world", new Dictionary<string, object> { { "number", world } });
		if (num != 0)
		{
			text += "?";
		}
		worldName.text = text;
		if (speedTrial)
		{
			int num2 = Mathf.RoundToInt((float)worldName.fontSize * 2f / 3f);
			Text text2 = worldName;
			text2.text = text2.text + "\n<size=" + num2 + ">" + RDString.Get("levelSelect.SpeedTrial") + "</size>";
		}
	}

	public void UpdateLanterns(string world)
	{
		bool coopMode = scrController.coopMode;
		if (RDC.forceUnlockAllLevels)
		{
			RDC.forceUnlockAllLevels = false;
		}
		int index = ADOBase.worldData[world].index;
		bool flag = Persistence.IsWorldComplete(index, coopMode, ignoreFools: false);
		bool isWorldPerfect = Persistence.IsWorldPerfect(index, coopMode);
		bool isWorldSpeedTrial = Persistence.IsSpeedTrialComplete(index, coopMode) && flag;
		RDC.forceUnlockAllLevels = Persistence.unlockAllLevels;
		lanterns.UpdateStates(flag, isWorldPerfect, isWorldSpeedTrial);
	}
}
