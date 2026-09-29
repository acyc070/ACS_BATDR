using System;

public static class SeasonalCheck
{
	public static bool HasHat;

	public static bool IsHatOn;

	public static bool IsSeasonal()
	{
		bool result;
		result = false;
		if (SeasonalCheck.GetCurrentSeason() != SeasonalType.None)
		{
			result = true;
		}
		return result;
	}

	public static bool IsSeasonal(out SeasonalType seasonalType)
	{
		bool result;
		result = false;
		seasonalType = SeasonalCheck.GetCurrentSeason();
		if (seasonalType != SeasonalType.None)
		{
			result = true;
		}
		return result;
	}

	public static SeasonalType GetCurrentSeason()
	{
		SeasonalType result;
		result = SeasonalType.None;
		DateTime now;
		now = DateTime.Now;
		if (now.Month == 2 && now.Day > 10 && now.Day <= 15)
		{
			result = (SeasonalType)1;
		}
		else if (now.Month == 10 && now.Day >= 23)
		{
			result = SeasonalType.InkDemonsEve;
		}
		else if (now.Month == 11 && now.Day >= 20 && now.Day <= 29)
		{
			result = (SeasonalType)3;
		}
		else if (now.Month == 12 && now.Day <= 26)
		{
			result = SeasonalType.WinterAbyss;
		}
		return result;
	}
}
