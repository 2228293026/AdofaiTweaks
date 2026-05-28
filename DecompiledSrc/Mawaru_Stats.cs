using System.Collections.Generic;

public static class Mawaru_Stats
{
	public static int checkpointsUsed = 0;

	public static int goals = 0;

	public static int hifive = 0;

	public static int coins = 0;

	public static bool apologized = false;

	public static List<int> sectionStats = new List<int>();

	public static int finalScore = 0;

	public static bool init = false;

	public static void Reset()
	{
		init = true;
		checkpointsUsed = 0;
		goals = 0;
		hifive = 0;
		coins = 0;
		apologized = false;
		sectionStats = new List<int>
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};
		finalScore = 0;
	}
}
