public class ffxCamInst : ffxPlusBase
{
	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		cam.ViewObjectInstant(ctrl.chosenPlanet.transform);
	}
}
