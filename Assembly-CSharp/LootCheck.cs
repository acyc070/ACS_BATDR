public static class LootCheck
{
	public static float GetChance()
	{
		float result;
		result = 0.8f;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 0.85f;
			break;
		case DifficultyLevel.Normal:
			result = 0.75f;
			break;
		case DifficultyLevel.Hard:
			result = 0.65f;
			break;
		case DifficultyLevel.Impossible:
			result = 0.5f;
			break;
		}
		return result;
	}
}
