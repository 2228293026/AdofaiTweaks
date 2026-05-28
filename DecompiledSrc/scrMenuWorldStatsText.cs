using System;
using System.Collections.Generic;
using MobileMenu;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuWorldStatsText : ADOBase
{
	public scrPortal portal;

	public Text text;

	private bool didUpdateText;

	private bool lastSpeedTrialState;

	private void Start()
	{
		text.SetLocalizedFont();
		UpdateText(portal.locked, GCS.speedTrialMode);
	}

	public void UpdateText(bool locked, bool speedTrial = false)
	{
		if (!scrController.instance.setupComplete)
		{
			return;
		}
		this.text.color = Color.white;
		MobileMenuController instance = MobileMenuController.instance;
		string newValue = "<color=#00aaff>";
		string newValue2 = "<color=#cccc00>";
		string text = "<color=#a8f0ff>";
		string text2 = "</color>";
		string text3 = (instance ? "    " : "\n");
		string world = portal.world;
		int index = ADOBase.worldData[world].index;
		bool coopMode = scrController.coopMode;
		Persistence.GetWorldAttempts(index);
		int worldAttempts = Persistence.GetWorldAttempts(index, coopMode);
		double num;
		double num2;
		int num3;
		bool num4;
		if (portal.locked)
		{
			bool exists = false;
			string text4 = "";
			text4 = ((!speedTrial) ? RDString.GetWithCheck("world" + world + ".requirement", out exists) : RDString.GetWithCheck("levelSelect.completeForTrial", out exists));
			if (exists)
			{
				this.text.text = text4;
			}
			else
			{
				this.text.text = "";
			}
		}
		else if (Persistence.GetLevelTutorialProgress(index) <= 0 && worldAttempts == 0 && !ADOBase.isMobile)
		{
			string text5 = RDString.Get("levelSelect.neverPlayed");
			this.text.text = (instance ? (text + text5 + text2) : text5);
		}
		else if (worldAttempts <= 0)
		{
			this.text.text = "";
		}
		else if (!Persistence.IsWorldComplete(index, coopMode, ignoreFools: false))
		{
			string value = (100f * Persistence.GetPercentCompletion(index, coopMode)).ToString("0.00");
			string[] array = RDString.Get("levelSelect.worldStatsIncomplete", new Dictionary<string, object>
			{
				{ "pctComplete", value },
				{ "numAttempts", worldAttempts }
			}).Split('\n', StringSplitOptions.None);
			string text6 = array[0].Replace("[[", newValue).Replace("]]", text2);
			string text7 = array[1].Replace("[[", newValue2).Replace("]]", text2);
			this.text.text = text6 + text3 + text7;
		}
		else
		{
			if (Persistence.GetBestPercentAccuracy(index, coopMode) != 0f)
			{
				num = Persistence.GetBestPercentAccuracy(index, coopMode);
				num2 = Persistence.GetBestPercentXAccuracy(index, coopMode);
				if (Persistence.showXAccuracy)
				{
					num3 = ((num2 != 0.0) ? 1 : 0);
					if (num3 != 0)
					{
						num4 = num2 == 1.0;
						goto IL_02a7;
					}
				}
				else
				{
					num3 = 0;
				}
				num4 = Persistence.GetIsHighestPossibleAcc(index, coopMode);
				goto IL_02a7;
			}
			this.text.text = RDString.Get("levelSelect.worldStatsCompleteCheckpoint").Replace("[[", newValue).Replace("]]", text2);
		}
		goto IL_044f;
		IL_044f:
		if (ADOBase.isMobileMenu && Persistence.ShouldShowSpeedTrials() && Persistence.IsWorldComplete(index) && speedTrial)
		{
			float num5 = Mathf.Max(Persistence.GetBestSpeedMultiplier(index, coopMode), 1f);
			this.text.text = RDString.Get("levelSelect.SpeedTrialBest", new Dictionary<string, object> { 
			{
				"bestMultiplier",
				num5.ToString("0.0")
			} });
			if (Persistence.IsSpeedTrialComplete(index, coopMode))
			{
				this.text.color = ADOBase.gc.goldTextColor;
				return;
			}
			string value2 = Persistence.GetSpeedTrialAimForWorld(world).ToString("0.0");
			Text obj = this.text;
			obj.text = obj.text + " ~ " + RDString.Get("levelSelect.SpeedTrialAim", new Dictionary<string, object> { { "aimMultiplier", value2 } });
		}
		return;
		IL_02a7:
		bool flag = num4;
		string value3 = ((num3 == 0) ? ((num > 1.0) ? ("100% + " + (100.0 * (num - 1.0)).ToString("0.00")) : (100.0 * num).ToString("0.00")) : (100.0 * num2).ToString("0.00"));
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "numAttempts", worldAttempts },
			{ "pctAccuracy", value3 }
		};
		string[] array2 = ((num3 != 0) ? RDString.Get("levelSelect.worldStatsCompleteXAccuracy", parameters) : RDString.Get("levelSelect.worldStatsComplete", parameters)).Split('\n', StringSplitOptions.None);
		string text8 = array2[0].Replace("[[", newValue).Replace("]]", text2);
		string text9 = array2[1].Replace("[[", newValue2).Replace("]]", text2);
		string text10 = array2[2].Replace("[[", newValue2).Replace("]]", text2);
		string text11 = array2[3];
		if (flag)
		{
			text11 = "<color=#FFDA00>" + text11 + "</color>";
		}
		string text12 = (instance ? (text8 + text3 + text10 + " " + text11) : (text8 + text3 + text9 + text3 + text3 + text10 + text3 + text11));
		this.text.text = text12;
		goto IL_044f;
	}
}
