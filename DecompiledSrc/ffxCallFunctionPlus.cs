using ByteSheep.Events;

public class ffxCallFunctionPlus : ffxPlusBase
{
	public QuickEvent ue;

	public override void Awake()
	{
		base.Awake();
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (base.enabled)
		{
			ue.Invoke();
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
