using ADOFAI;

public class ffxSetHoldsound : ffxPlusBase
{
	public HoldStartSound holdStartSound;

	public HoldLoopSound holdLoopSound;

	public HoldEndSound holdEndSound;

	public HoldMidSound holdMidSound;

	public HoldMidSoundType holdMidSoundType;

	public float holdMidSoundDelay;

	public HoldMidSoundTimingRelativeTo holdMidSoundTiming;

	public float volume;

	public override bool runOnHit => true;

	public override void Decode(LevelEvent evnt)
	{
		holdStartSound = (HoldStartSound)evnt["holdStartSound"];
		holdLoopSound = (HoldLoopSound)evnt["holdLoopSound"];
		holdEndSound = (HoldEndSound)evnt["holdEndSound"];
		holdMidSound = (HoldMidSound)evnt["holdMidSound"];
		holdMidSoundType = (HoldMidSoundType)evnt["holdMidSoundType"];
		holdMidSoundDelay = evnt.GetFloat("holdMidSoundDelay") * crotchet;
		holdMidSoundTiming = (HoldMidSoundTimingRelativeTo)evnt["holdMidSoundTimingRelativeTo"];
		volume = (float)evnt.GetInt("holdSoundVolume") / 100f;
	}

	public override void StartEffect(scrPlanet planet)
	{
	}
}
