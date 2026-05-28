using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class scrMarginTracker
{
	public List<HitMargin> hitMargins = new List<HitMargin>();

	public int[] hitMarginsCount = new int[Enum.GetValues(typeof(HitMargin)).Cast<int>().Max() + 1];

	public Difficulty hardestDifficulty;

	public int lastHitMarginsSize;

	public int deadTilesBeforeCheckpoint;

	public int deadTiles;

	public int deadTilesStartFloor;

	public int deathsBeforeCheckpoint;

	public int deaths;

	public scrController controller => scrController.instance;

	public scrLevelMaker lm => scrLevelMaker.instance;

	public scnGame customLevel => scnGame.instance;

	public float percentComplete { get; private set; }

	public float percentAcc { get; private set; }

	public float percentXAcc { get; private set; }

	public void RevertToLastCheckpoint()
	{
		while (hitMargins.Count > lastHitMarginsSize)
		{
			HitMargin hitMargin = hitMargins.Pop();
			hitMarginsCount[(int)hitMargin]--;
		}
		deadTiles = deadTilesBeforeCheckpoint;
		deaths = deathsBeforeCheckpoint;
	}

	public void MarkCheckpoint(int checkpointTileOffset)
	{
		lastHitMarginsSize = hitMargins.Count;
		int num = 0;
		while (lastHitMarginsSize > 0)
		{
			if (hitMargins[lastHitMarginsSize - 1] != HitMargin.TooEarly)
			{
				if (num >= -checkpointTileOffset)
				{
					break;
				}
				num++;
			}
			lastHitMarginsSize--;
		}
		deadTilesBeforeCheckpoint = deadTiles;
		deathsBeforeCheckpoint = deaths;
	}

	public void MarkDeath(int floorID)
	{
		deadTilesStartFloor = floorID;
		deaths++;
	}

	public void RegisterDeadTiles(int floorID)
	{
		int num = 0;
		for (int i = deadTilesStartFloor; i < floorID; i++)
		{
			scrFloor scrFloor2 = lm.listFloors[i];
			if (!scrFloor2.auto && !scrFloor2.midSpin)
			{
				num++;
			}
		}
		deadTiles += num;
		deadTilesStartFloor = floorID;
		UpdateHardestDifficulty();
	}

	public void Reset()
	{
		hitMargins = new List<HitMargin>();
		lastHitMarginsSize = 0;
		for (int i = 0; i < hitMarginsCount.Length; i++)
		{
			hitMarginsCount[i] = 0;
		}
		hardestDifficulty = Difficulty.Strict;
		deadTilesBeforeCheckpoint = 0;
		deadTiles = 0;
		deadTilesStartFloor = 0;
		deathsBeforeCheckpoint = 0;
		deaths = 0;
		CalculatePercentAcc();
	}

	public void AddHit(HitMargin hit)
	{
		hitMargins.Add(hit);
		hitMarginsCount[(int)hit]++;
		CalculatePercentAcc();
		UpdateHardestDifficulty();
	}

	private void UpdateHardestDifficulty()
	{
		hardestDifficulty = (Difficulty)Mathf.Min((int)GCS.difficulty, (int)hardestDifficulty);
	}

	public int GetHits(HitMargin hitMargin)
	{
		return hitMarginsCount[(int)hitMargin];
	}

	public int GetHits(params HitMargin[] hitMargins)
	{
		int num = 0;
		foreach (HitMargin hitMargin in hitMargins)
		{
			num += GetHits(hitMargin);
		}
		return num;
	}

	public int GetDeaths()
	{
		return GetHits(HitMargin.FailMiss, HitMargin.FailOverload);
	}

	public float GetTotalHits()
	{
		return hitMargins.Count;
	}

	public void CalculatePercentAcc()
	{
		double num = GetHits(HitMargin.Perfect, HitMargin.EarlyPerfect, HitMargin.LatePerfect, HitMargin.Auto);
		double num2 = hitMargins.Count + GetHits(HitMargin.FailMiss, HitMargin.FailOverload);
		double num3 = ((num == num2) ? 1.0 : (num / num2));
		double num4 = (double)GetHits(HitMargin.Perfect, HitMargin.Auto) * 0.0001;
		num4 -= (double)deadTiles * 0.0001;
		percentAcc = (float)(num3 + num4);
		if ((bool)lm && (bool)scrController.instance)
		{
			percentComplete = (float)(scrController.instance.currentSeqID + 1) / (float)lm.listFloors.Count;
		}
		double num5 = (1.0 * (double)GetHits(HitMargin.Perfect, HitMargin.Auto) + 0.75 * (double)GetHits(HitMargin.EarlyPerfect, HitMargin.LatePerfect) + 0.4 * (double)GetHits(HitMargin.VeryEarly, HitMargin.VeryLate) + 0.2 * (double)GetHits(HitMargin.TooEarly, HitMargin.TooLate) + 0.2 * (double)deadTiles) / (double)(hitMargins.Count + deadTiles);
		percentXAcc = (float)(num5 * Math.Pow(0.9875, scrController.checkpointsUsed));
		controller.playerManager.mistakesManager.CalculateTotalAccuracy();
	}

	public bool IsAllPurePerfect()
	{
		if (controller.startedFromCheckpoint)
		{
			return false;
		}
		if (deadTiles > 0)
		{
			return false;
		}
		foreach (HitMargin hitMargin in hitMargins)
		{
			if (hitMargin != HitMargin.Perfect && hitMargin != HitMargin.Auto)
			{
				return false;
			}
		}
		return true;
	}
}
