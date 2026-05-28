using System;

public class ffxMDEnemyHit : ffxPlusBase
{
	public Action onHit;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		onHit();
	}
}
