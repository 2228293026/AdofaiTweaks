public class ffxCamSetParams : ffxPlusBase
{
	public float size;

	public float tweenDuration;

	public bool isPulsing;

	public bool isMoving;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		cam.isPulsingOnHit = isPulsing;
		cam.isMoveTweening = isMoving;
		if (tweenDuration == 0f)
		{
			cam.camsizenormal = size;
		}
		else
		{
			cam.setCamSizeSmooth(size, tweenDuration);
		}
	}
}
