public static class SeasonalSkullCheck
{
	public static int SkullCount;

	public static void Collect()
	{
		unchecked
		{
			SeasonalSkullCheck.SkullCount++;
		}
	}

	public static void Reset()
	{
		SeasonalSkullCheck.SkullCount = 0;
	}
}
