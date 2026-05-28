using DG.Tweening;

public class ffxScriptEnable : ffxPlusBase
{
	public DOTweenPath path;

	public bool enable;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		if (path != null)
		{
			if (enable)
			{
				path.DOPlay();
			}
			else
			{
				path.DOPause();
			}
		}
	}
}
