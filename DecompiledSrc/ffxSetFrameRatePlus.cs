using ADOFAI;

public class ffxSetFrameRatePlus : ffxPlusBase
{
	public bool enableCustomFrameRate;

	public float frameRate;

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		cam.SetCustomFrameRate(enableCustomFrameRate, frameRate);
	}

	public override void Decode(LevelEvent evnt)
	{
		enableCustomFrameRate = evnt.GetBool("enabled");
		frameRate = evnt.GetFloat("frameRate");
	}
}
