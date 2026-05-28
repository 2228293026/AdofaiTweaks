public class ffxPulseMag : ffxPlusBase
{
	public float pulsemag;

	public bool justOnce;

	private float oldPulseMag;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			return;
		}
		cam.isPulsingOnHit = true;
		oldPulseMag = cam.pulsemagnitude;
		cam.pulsemagnitude = pulsemag;
		if (justOnce)
		{
			Timer.Add(delegate
			{
				cam.pulsemagnitude = oldPulseMag;
			}, 0.03f);
		}
	}
}
