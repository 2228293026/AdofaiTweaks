public static class EnumExtensions
{
	public static bool IsFail(this HitMargin margin)
	{
		if (margin != HitMargin.TooLate && margin != HitMargin.FailMiss)
		{
			return margin == HitMargin.FailOverload;
		}
		return true;
	}

	public static bool IsMiss(this HitMargin margin)
	{
		if (margin != HitMargin.VeryEarly)
		{
			return margin == HitMargin.VeryLate;
		}
		return true;
	}

	public static bool IsAnyPerfect(this HitMargin margin)
	{
		if (margin != HitMargin.Perfect && margin != HitMargin.EarlyPerfect)
		{
			return margin == HitMargin.LatePerfect;
		}
		return true;
	}
}
