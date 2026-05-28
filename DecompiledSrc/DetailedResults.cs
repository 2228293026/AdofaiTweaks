using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DetailedResults : ADOBase
{
	public Text textComponent;

	public scrHUDText hudText;

	private string[] results;

	private float timer;

	private int currentPlayerIndex;

	private string GenerateResults(scrMarginTracker marginTracker)
	{
		bool isPurePerfect = marginTracker.IsAllPurePerfect();
		int resultCount = GetHits(new HitMargin[1] { HitMargin.Perfect });
		int resultCount2 = GetHits(new HitMargin[1] { HitMargin.EarlyPerfect });
		int resultCount3 = GetHits(new HitMargin[1] { HitMargin.LatePerfect });
		int resultCount4 = GetHits(new HitMargin[1] { HitMargin.VeryEarly });
		int resultCount5 = GetHits(new HitMargin[1] { HitMargin.VeryLate });
		int resultCount6 = GetHits(new HitMargin[1]);
		int resultCount7 = (scrController.coopMode ? marginTracker.deaths : GetHits(new HitMargin[1] { HitMargin.FailMiss }));
		int resultCount8 = GetHits(new HitMargin[1] { HitMargin.FailOverload });
		int num = GetHits(new HitMargin[1] { HitMargin.Auto });
		float num2 = (Persistence.showXAccuracy ? marginTracker.percentXAcc : marginTracker.percentAcc);
		ColourSchemeHitMargin hitMarginColoursUI = RDConstants.data.hitMarginColoursUI;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Localized("ePerfect")).Append(Result(resultCount2, hitMarginColoursUI.colourLittleEarly.ToHex())).Append("     ");
		if (num > 0 && !ADOBase.isOfficialLevel)
		{
			stringBuilder.Append(Localized("perfect")).Append(ResultWithAuto(resultCount, num, hitMarginColoursUI.colourPerfect.ToHex())).Append("     ");
		}
		else
		{
			stringBuilder.Append(Localized("perfect")).Append(Result(resultCount, hitMarginColoursUI.colourPerfect.ToHex())).Append("     ");
		}
		stringBuilder.Append(Localized("lPerfect")).Append(Result(resultCount3, hitMarginColoursUI.colourLittleLate.ToHex())).Append("\n");
		stringBuilder.Append(Localized("tooEarly")).Append(Result(resultCount6, hitMarginColoursUI.colourTooEarly.ToHex())).Append("     ");
		stringBuilder.Append(Localized("early")).Append(Result(resultCount4, hitMarginColoursUI.colourVeryEarly.ToHex())).Append("     ");
		stringBuilder.Append(Localized("late")).Append(Result(resultCount5, hitMarginColoursUI.colourVeryLate.ToHex())).Append("\n");
		if (ADOBase.controller.noFail || ADOBase.controller.safetyTilesArePresent || scrController.coopMode)
		{
			stringBuilder.Append(Localized("missFails")).Append(Result(resultCount7, hitMarginColoursUI.colourFail.ToHex())).Append("     ");
			stringBuilder.Append(Localized("overloadFails")).Append(Result(resultCount8, hitMarginColoursUI.colourFail.ToHex())).Append("\n");
		}
		stringBuilder.Append(Localized(Persistence.showXAccuracy ? "xAccuracy" : "accuracy")).Append(GoldAccuracy($"{num2 * 100f:0.00}%")).Append("     ");
		stringBuilder.Append(GCS.practiceMode ? Localized("practiceAttempts") : Localized("checkpoints")).Append(scrController.checkpointsUsed.ToString()).Append("\n");
		if (ADOBase.controller.maximumUsedKeys > 10)
		{
			stringBuilder.Append(Localized("maximumUsedKeys")).Append(ADOBase.controller.maximumUsedKeys.ToString()).Append("\n");
		}
		return stringBuilder.ToString();
		int GetHits(HitMargin[] hitMargins)
		{
			return marginTracker.GetHits(hitMargins);
		}
		string GoldAccuracy(string accText)
		{
			if (!isPurePerfect)
			{
				return accText;
			}
			return "<color=#FFDA00>" + accText + "</color>";
		}
		static string Localized(string s)
		{
			return RDString.Get("status.results." + s) + ": ";
		}
		static string Result(int num3, string color)
		{
			return $"<color={color}>{num3}</color>";
		}
		static string ResultWithAuto(int num3, int num4, string color)
		{
			return $"<color={color}>{num3}({num4})</color>";
		}
	}

	public void Show()
	{
		results = new string[scrPlayerManager.playerCount];
		for (int i = 0; i < scrPlayerManager.playerCount; i++)
		{
			scrPlayer scrPlayer2 = ADOBase.controller.playerManager.players[i];
			results[i] = GenerateResults(scrPlayer2.marginTracker);
		}
		base.enabled = true;
		timer = 0f;
		ADOBase.controller.detailedResults.gameObject.SetActive(value: true);
		ShowForPlayer(0);
	}

	private void Update()
	{
		if (!scrController.coopMode)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= 2f)
		{
			Debug.Log(timer);
			timer = 0f;
			int num = currentPlayerIndex;
			currentPlayerIndex++;
			currentPlayerIndex %= scrPlayerManager.playerCount;
			if (num != currentPlayerIndex)
			{
				ShowForPlayer(currentPlayerIndex);
			}
		}
	}

	private void ShowForPlayer(int playerIndex)
	{
		if (scrController.coopMode)
		{
			hudText.enabled = false;
			Color color = ADOBase.controller.playerManager.players[playerIndex].planetarySystem.chosenPlanet.planetRenderer.planetColor.ToRealColor();
			textComponent.color = color;
		}
		textComponent.text = results[playerIndex];
	}
}
