public class ffxSetOffset : ffxPlusBase
{
	public float xoffset;

	public float yoffset;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		cam.SetYOffset(yoffset);
		cam.SetXOffset(xoffset);
	}
}
