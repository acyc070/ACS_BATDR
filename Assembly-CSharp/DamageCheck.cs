public static class DamageCheck
{
	public static int ThrowableGear()
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

	public static int ProjectileInk()
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
			result = 1;
			break;
		case DifficultyLevel.Impossible:
			result = 2;
			break;
		}
		return result;
	}
}
