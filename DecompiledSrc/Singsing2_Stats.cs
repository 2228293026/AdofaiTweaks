using System.Collections.Generic;

public static class Singsing2_Stats
{
	public static int checkpointsUsed = 0;

	public static List<int> sectionStats = new List<int>();

	public static bool init = false;

	public static void Reset()
	{
		init = true;
		checkpointsUsed = 0;
		sectionStats = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
	}
}
