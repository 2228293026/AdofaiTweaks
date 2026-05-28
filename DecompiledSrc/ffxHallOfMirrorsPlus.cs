using ADOFAI;

public class ffxHallOfMirrorsPlus : ffxPlusBase
{
	public bool enableHOM;

	public override void Awake()
	{
		base.Awake();
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		ADOBase.controller.EnableHallOfMirrors(enableHOM);
	}

	public override void Decode(LevelEvent evnt)
	{
		enableHOM = evnt.GetBool("enabled");
	}
}
