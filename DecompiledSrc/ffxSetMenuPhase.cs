public class ffxSetMenuPhase : ffxPlusBase
{
	public int phase;

	public override void StartEffect(scrPlanet planet)
	{
		ADOBase.levelSelectBase.menuPhase = phase;
	}
}
