public static class PresentCheck
{
	public static int PresentCount;

	public static void Collect()
	{
		unchecked
		{
			PresentCheck.PresentCount++;
		}
	}

	public static void Reset()
	{
		PresentCheck.PresentCount = 0;
	}
}
