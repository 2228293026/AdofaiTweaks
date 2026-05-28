public class ffxGlitchFilter : ffxPlusBase
{
	public bool enable;

	public override void StartEffect(scrPlanet planet)
	{
		cam.GetComponent<CameraFilterPack_FX_Glitch1>().enabled = enable;
	}
}
