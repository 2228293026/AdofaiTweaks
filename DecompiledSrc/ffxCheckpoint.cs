using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxCheckpoint : ffxPlusBase
{
	public int checkpointTileOffset;

	public bool scrubFourBack;

	public override bool runOnHit => true;

	public override void Awake()
	{
		base.Awake();
		if (floor.floorIcon == FloorIcon.None || floor.floorIcon == FloorIcon.Vfx)
		{
			floor.floorIcon = ((!GCS.speedTrialMode && !GCS.practiceMode && GCS.hitMarginLimit != HitMarginLimit.PurePerfectOnly) ? FloorIcon.Checkpoint : FloorIcon.None);
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (GCS.speedTrialMode || GCS.practiceMode || GCS.hitMarginLimit == HitMarginLimit.PurePerfectOnly)
		{
			return;
		}
		int num = floor.seqID + checkpointTileOffset;
		if (num <= GCS.checkpointNum)
		{
			return;
		}
		int num2 = 0;
		scrPlayer[] players = ADOBase.controller.playerManager.players;
		foreach (scrPlayer scrPlayer2 in players)
		{
			if (!scrPlayer2.alive)
			{
				scrPlayer2.Revive(Mathf.Max(floorID, num), planet.player);
				num2++;
			}
		}
		if (num2 > 0)
		{
			scrSfx.instance.PlaySfx(SfxSound.PlanetRevive, MixerGroup.SfxParent);
		}
		scrFlash.Flash(Color.white.WithAlpha(0.3f));
		GCS.checkpointNum = num;
		ADOBase.controller.mistakesManager.MarkCheckpoint(checkpointTileOffset);
		floor.floorIcon = FloorIcon.Checkpoint;
		floor.UpdateIconSprite();
	}

	public override void Decode(LevelEvent evnt)
	{
		int num = 0;
		if (evnt.TryGet<int>("tileOffset", out var output))
		{
			num = output;
			if (floor.seqID + num < 0)
			{
				num = -floor.seqID;
			}
			if (floor.seqID + num > ADOBase.lm.listFloors.Count - 2)
			{
				num = ADOBase.lm.listFloors.Count - 2 - floor.seqID;
			}
		}
		checkpointTileOffset = num;
		ease = Ease.InCirc;
	}
}
