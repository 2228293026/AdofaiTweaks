using ADOFAI;

public class ffxKillPlayer : ffxPlusBase
{
	public bool instant;

	public string failMessage;

	public override void StartEffect(scrPlanet planet)
	{
		if (RDC.auto || ctrl.currentState >= States.Fail || ctrl.noFail)
		{
			return;
		}
		bool flag = false;
		bool[] array = conditionalInfo;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				flag = true;
			}
		}
		if (flag)
		{
			ctrl.instantExplode = instant;
			ctrl.playerOne.Die(overload: false, multipress: false, failMessage);
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		instant = !evnt.GetBool("playAnimation");
		failMessage = evnt.GetStringLocalized("failMessage");
	}
}
