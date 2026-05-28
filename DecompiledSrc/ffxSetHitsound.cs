using ADOFAI;
using UnityEngine;

public class ffxSetHitsound : ffxPlusBase
{
	public GameSound gameSound;

	public HitSound hitSound = HitSound.ReverbClack;

	[Range(0f, 1f)]
	public float volume = 1f;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
	}

	public override void Decode(LevelEvent evnt)
	{
		gameSound = (GameSound)evnt["gameSound"];
		hitSound = (HitSound)evnt["hitsound"];
		volume = (float)evnt.GetInt("hitsoundVolume") / 100f;
		floor.setHitsound = this;
	}
}
