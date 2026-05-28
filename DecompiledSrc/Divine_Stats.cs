using System.Collections.Generic;

public static class Divine_Stats
{
	public static float bestTimeTrialTime = 60f;

	public static float timeTrialTime = 60f;

	public static List<int> sectionStats = new List<int>();

	public static bool init = false;

	public static void Reset()
	{
		init = true;
		timeTrialTime = 60f;
		bestTimeTrialTime = Persistence.t5BestTime;
		sectionStats = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	}
}
