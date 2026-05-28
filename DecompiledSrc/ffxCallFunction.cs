using ByteSheep.Events;

public class ffxCallFunction : ffxPlusBase
{
	public QuickEvent ue;

	public override void StartEffect(scrPlanet planet)
	{
		if (base.enabled)
		{
			ue.Invoke();
		}
	}
}
