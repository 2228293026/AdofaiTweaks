using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class scrMistakesManager
{
	public struct EndLevelInfo
	{
		public EndLevelType endLevelType;

		public NewBestType newBestType;
	}

	public static scrMarginTracker[] marginTrackers = new scrMarginTracker[1]
	{
		new scrMarginTracker()
	};

	private readonly float[] accuracyWeights = new float[4] { 64f, 16f, 12f, 8f };

	public scrController controller => scrController.instance;

	public scrLevelMaker lm => scrLevelMaker.instance;

	public scnGame customLevel => scnGame.instance;

	public static Difficulty hardestDifficulty => marginTrackers.Select((scrMarginTracker x) => x.hardestDifficulty).Max();

	public float percentComplete { get; private set; }

	public float percentAcc { get; private set; }

	public float percentXAcc { get; private set; }

	public static void SetPlayerCount(int playerCount)
	{
		marginTrackers = new scrMarginTracker[playerCount];
		for (int i = 0; i < playerCount; i++)
		{
			marginTrackers[i] = new scrMarginTracker();
		}
	}

	public void CalculateTotalAccuracy()
	{
		if (!scrController.coopMode)
		{
			scrMarginTracker marginTracker = controller.playerOne.marginTracker;
			percentComplete = marginTracker.percentComplete;
			percentAcc = marginTracker.percentAcc;
			percentXAcc = marginTracker.percentXAcc;
			return;
		}
		float[] array = (from x in controller.playerManager.players
			select x.marginTracker.percentAcc into x
			orderby x descending
			select x).ToArray();
		float[] array2 = (from x in controller.playerManager.players
			select x.marginTracker.percentXAcc into x
			orderby x descending
			select x).ToArray();
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int num4 = 0; num4 < array.Length; num4++)
		{
			float num5 = accuracyWeights[num4];
			num += array[num4] * num5;
			num2 += array2[num4] * num5;
			num3 += num5;
		}
		percentAcc = num / num3;
		percentXAcc = num2 / num3;
		percentComplete = controller.playerManager.players.Select((scrPlayer x) => x.marginTracker.percentComplete).Max();
	}

	public EndLevelInfo SaveCustom(string hash, bool wonLevel, float multiplier)
	{
		if (scrController.coopMode)
		{
			throw new NotImplementedException("Co-op scoring for customs is not yet implemented!");
		}
		EndLevelInfo result = new EndLevelInfo
		{
			endLevelType = EndLevelType.None,
			newBestType = NewBestType.None
		};
		if (controller.noFail && controller.playerOne.marginTracker.GetDeaths() > 0)
		{
			return result;
		}
		if (controller.unlockKeyLimiter && controller.maximumUsedKeys > 1000)
		{
			return result;
		}
		float customWorldAccuracy = Persistence.GetCustomWorldAccuracy(hash);
		float customWorldXAccuracy = Persistence.GetCustomWorldXAccuracy(hash);
		float customWorldCompletion = Persistence.GetCustomWorldCompletion(hash);
		bool customWorldIsHighestPossibleAcc = Persistence.GetCustomWorldIsHighestPossibleAcc(hash);
		bool flag = controller.playerOne.marginTracker.IsAllPurePerfect();
		if (multiplier >= 1f)
		{
			if (!wonLevel)
			{
				if (percentComplete > customWorldCompletion && controller.currentFloorID > 0)
				{
					result.endLevelType = EndLevelType.NewBest;
					result.newBestType = NewBestType.Regular;
					Persistence.SetCustomWorldCompletion(hash, percentComplete);
				}
			}
			else
			{
				float customWorldSpeedTrial = Persistence.GetCustomWorldSpeedTrial(hash);
				int customWorldMinDeaths = Persistence.GetCustomWorldMinDeaths(hash);
				Persistence.SetCustomWorldCompletion(hash, 1f);
				if (customWorldCompletion < 1f && multiplier == 1f)
				{
					result.endLevelType = EndLevelType.FirstWin;
				}
				else if (GCS.speedTrialMode && multiplier >= 1f && multiplier > customWorldSpeedTrial)
				{
					result.endLevelType = EndLevelType.FirstWinSpeedTrial;
					Persistence.SetCustomWorldSpeedTrial(hash, multiplier);
				}
				if (percentAcc > customWorldAccuracy)
				{
					Persistence.SetCustomWorldAccuracy(hash, percentAcc);
					if (flag)
					{
						Persistence.SetCustomWorldIsHighestPossibleAcc(hash, isHighest: true);
					}
				}
				if (percentXAcc > customWorldXAccuracy)
				{
					Persistence.SetCustomWorldXAccuracy(hash, percentXAcc);
				}
				if (customLevel.checkpointsUsed < customWorldMinDeaths || customWorldMinDeaths == -1)
				{
					Persistence.SetCustomWorldMinDeaths(hash, customLevel.checkpointsUsed);
				}
				if (!customWorldIsHighestPossibleAcc && flag)
				{
					Persistence.SetCustomWorldIsHighestPossibleAcc(hash, isHighest: true);
				}
				if (ADOBase.isTechFeaturedLevel)
				{
					Persistence.clearedTechFeatured = true;
				}
			}
			Persistence.Save();
		}
		return result;
	}

	public EndLevelInfo Save(int worldZeroIndexed, bool wonLevel, float multiplier)
	{
		EndLevelInfo result = new EndLevelInfo
		{
			endLevelType = EndLevelType.None,
			newBestType = NewBestType.None
		};
		bool coopMode = scrController.coopMode;
		if (!scrController.coopMode && controller.noFail && controller.playerOne.marginTracker.GetDeaths() > 0)
		{
			return result;
		}
		if (controller.unlockKeyLimiter)
		{
			return result;
		}
		float percentCompletion = Persistence.GetPercentCompletion(worldZeroIndexed, coopMode);
		float bestPercentAccuracy = Persistence.GetBestPercentAccuracy(worldZeroIndexed, coopMode);
		float bestPercentXAccuracy = Persistence.GetBestPercentXAccuracy(worldZeroIndexed, coopMode);
		float bestSpeedMultiplier = Persistence.GetBestSpeedMultiplier(worldZeroIndexed, coopMode);
		string currentWorldString = scrController.currentWorldString;
		if (!wonLevel)
		{
			if (percentComplete > percentCompletion && controller.currentFloorID > 0)
			{
				result.endLevelType = EndLevelType.NewBest;
				result.newBestType = NewBestType.Regular;
				int worldAttemptsWithoutNewBest = Persistence.GetWorldAttemptsWithoutNewBest(worldZeroIndexed, coopMode);
				float num = percentComplete - percentCompletion;
				if (worldAttemptsWithoutNewBest >= 15)
				{
					result.newBestType = NewBestType.Applause;
				}
				else if (worldAttemptsWithoutNewBest >= 5 && num >= 0.01f)
				{
					result.newBestType = NewBestType.Jingle;
				}
				Persistence.SetWorldAttemptsWithoutNewBest(worldZeroIndexed, 0, coopMode);
				Persistence.SetPercentCompletion(worldZeroIndexed, percentComplete, coopMode);
			}
		}
		else
		{
			Persistence.SetPercentCompletion(worldZeroIndexed, 1f, coopMode);
			if (percentCompletion < 1f && multiplier == 1f)
			{
				result.endLevelType = EndLevelType.FirstWin;
				Persistence.SetPercentCompletion(worldZeroIndexed, 1f, coopMode);
			}
			else if (GCS.speedTrialMode && multiplier > bestSpeedMultiplier)
			{
				bool flag = Persistence.IsSpeedTrialComplete(worldZeroIndexed, coopMode);
				Persistence.SetBestSpeedTrial(worldZeroIndexed, multiplier, coopMode);
				if (multiplier >= GCNS.worldData[currentWorldString].trialAim)
				{
					bool flag2 = Persistence.IsSpeedTrialComplete(worldZeroIndexed, coopMode);
					if (!flag && flag2)
					{
						result.endLevelType = EndLevelType.FirstWinSpeedTrial;
					}
					else
					{
						result.endLevelType = EndLevelType.NewBestMult;
					}
				}
			}
			if (percentAcc > bestPercentAccuracy)
			{
				Persistence.SetBestPercentAccuracy(worldZeroIndexed, percentAcc, coopMode);
			}
			if (percentXAcc > bestPercentXAccuracy)
			{
				Persistence.SetBestPercentXAccuracy(worldZeroIndexed, percentXAcc, coopMode);
			}
			bool flag3 = true;
			foreach (scrPlayer item in controller.playerManager)
			{
				if (!item.marginTracker.IsAllPurePerfect())
				{
					flag3 = false;
				}
			}
			if (flag3)
			{
				Persistence.SetIsHighestPossibleAcc(worldZeroIndexed, coopMode, coop: true);
			}
		}
		Persistence.GiveAchievements();
		Persistence.Save();
		return result;
	}

	public static void SaveCheckpointProgress(bool save)
	{
		if (!scrController.coopMode && GCS.checkpointNum != 0)
		{
			scrMarginTracker marginTracker = scrController.instance.playerOne.marginTracker;
			Persistence.SetSavedProgress(new Dictionary<string, object>
			{
				["hitMargins"] = string.Join<int>(',', marginTracker.hitMargins.Select((HitMargin x) => (int)x)),
				["lastHitMarginsSize"] = marginTracker.lastHitMarginsSize,
				["checkpointNum"] = GCS.checkpointNum,
				["level"] = scrController.instance.levelName,
				["hardestDifficulty"] = marginTracker.hardestDifficulty.ToString(),
				["checkpointsUsed"] = scrController.checkpointsUsed
			});
			if (save)
			{
				Persistence.generalPrefs.Save();
			}
		}
	}

	public static void LoadCheckpointProgress()
	{
		if (scrController.coopMode)
		{
			return;
		}
		Dictionary<string, object> savedProgress = Persistence.GetSavedProgress();
		string text = savedProgress["hitMargins"] as string;
		if (text.IsNullOrEmpty() || !text.Contains(','))
		{
			Debug.LogError("Error with hitMarginsString value: " + text);
			return;
		}
		scrMarginTracker marginTracker = scrController.instance.playerOne.marginTracker;
		marginTracker.hitMargins.Clear();
		for (int i = 0; i < marginTracker.hitMarginsCount.Length; i++)
		{
			marginTracker.hitMarginsCount[i] = 0;
		}
		string[] array = text.Split(',', StringSplitOptions.None);
		for (int j = 0; j < array.Length; j++)
		{
			int num = Convert.ToInt32(array[j]);
			marginTracker.hitMargins.Add((HitMargin)num);
			marginTracker.hitMarginsCount[num]++;
		}
		GCS.checkpointNum = (int)savedProgress["checkpointNum"];
		marginTracker.lastHitMarginsSize = (int)savedProgress["lastHitMarginsSize"];
		if (savedProgress.TryGetValueAs<string, object, string>("hardestDifficulty", out var valueAs))
		{
			marginTracker.hardestDifficulty = RDUtils.ParseEnum(valueAs, Difficulty.Normal);
		}
		else
		{
			marginTracker.hardestDifficulty = Difficulty.Normal;
		}
		if (savedProgress.TryGetValueAs("checkpointsUsed", out var valueAs2, 0))
		{
			scrController.checkpointsUsed = valueAs2;
		}
		if (GCS.checkpointNum > 0)
		{
			scrController.checkpointsUsed++;
		}
	}

	public void Reset()
	{
		scrMarginTracker[] array = marginTrackers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Reset();
		}
	}

	public void RevertToLastCheckpoint()
	{
		scrMarginTracker[] array = marginTrackers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RevertToLastCheckpoint();
		}
	}

	public void MarkCheckpoint(int checkpointTileOffset)
	{
		scrMarginTracker[] array = marginTrackers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MarkCheckpoint(checkpointTileOffset);
		}
	}

	public bool IsAllPurePerfect()
	{
		scrMarginTracker[] array = marginTrackers;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsAllPurePerfect())
			{
				return false;
			}
		}
		return true;
	}
}
