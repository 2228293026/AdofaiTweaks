using System.Collections.Generic;

public class GameServicesEmpty : GameServices
{
	public override bool IsLoadStatusComplete => true;

	public override bool Initialized => false;

	public override bool TimeOut => true;

	public override bool IsFirstLoading => false;

	public override bool DebugIsPosible => false;

	public override List<int> FrameRates => new List<int> { 30, 60, 120 };

	public override int MaxFrameRate => 120;

	public override void Initialize()
	{
	}

	public override void UnlockAchievementWithName(string name)
	{
	}

	public override void Vibrate(long ms)
	{
	}

	public override void CancelVibration()
	{
	}
}
