public class ffxSpeed : ffxPlusBase
{
	public float speed = 1f;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		ADOBase.controller.d_speed = speed;
	}
}
