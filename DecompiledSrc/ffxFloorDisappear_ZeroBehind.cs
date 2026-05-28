public class ffxFloorDisappear_ZeroBehind : ffxPlusBase
{
	public ffxFloorDisappearPlus parent;

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && parent.hifiEffect && ADOBase.isOfficialLevel)
		{
			parent.StartEffect(planet);
		}
	}
}
