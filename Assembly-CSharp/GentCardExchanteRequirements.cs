public static class GentCardExchanteRequirements
{
	public static int GetBatteryCasings()
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 1;
			break;
		case DifficultyLevel.Normal:
			result = 1;
			break;
		case DifficultyLevel.Hard:
			result = 2;
			break;
		case DifficultyLevel.Impossible:
			result = 2;
			break;
		}
		return result;
	}

	public static int GetToolkits()
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 1;
			break;
		case DifficultyLevel.Normal:
			result = 2;
			break;
		case DifficultyLevel.Hard:
			result = 3;
			break;
		case DifficultyLevel.Impossible:
			result = 3;
			break;
		}
		return result;
	}

	public static int GetParts()
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 3;
			break;
		case DifficultyLevel.Normal:
			result = 4;
			break;
		case DifficultyLevel.Hard:
			result = 6;
			break;
		case DifficultyLevel.Impossible:
			result = 6;
			break;
		}
		return result;
	}
}
