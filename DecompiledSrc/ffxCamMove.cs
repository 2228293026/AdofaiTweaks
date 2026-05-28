public class ffxCamMove : ffxPlusBase
{
	public bool follow;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		cam.isMoveTweening = follow;
	}
}
