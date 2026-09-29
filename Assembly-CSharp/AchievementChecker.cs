public static class AchievementChecker
{
	public static void Check(AchievementName achievementName)
	{
		if (AchievementChecker.CanUnlock(achievementName))
		{
			AchievementChecker.GetAchievement(achievementName);
		}
	}

	private static bool CanUnlock(AchievementName achievementName)
	{
		bool result;
		result = true;
		switch (achievementName)
		{
		case AchievementName.SOCIALITE:
			if (GameManager.Instance.GameData.CurrentSave.Difficulty.IsNotSocialite)
			{
				result = false;
			}
			break;
		case AchievementName.THE_MASTERS_PEN:
		{
			DifficultyData difficulty;
			difficulty = GameManager.Instance.GameData.CurrentSave.Difficulty;
			if (!difficulty.DifficultyChanged)
			{
				if (difficulty.Difficulty == DifficultyLevel.Hard)
				{
					AchievementChecker.GetAchievement(AchievementName.STUDIO_STARTER);
					AchievementChecker.GetAchievement(AchievementName.STUDIO_SCRAPPER);
					AchievementChecker.GetAchievement(AchievementName.STUDIO_BREAKER);
				}
				else if (difficulty.Difficulty == DifficultyLevel.Normal)
				{
					AchievementChecker.GetAchievement(AchievementName.STUDIO_STARTER);
					AchievementChecker.GetAchievement(AchievementName.STUDIO_SCRAPPER);
				}
				else if (difficulty.Difficulty == DifficultyLevel.Easy)
				{
					AchievementChecker.GetAchievement(AchievementName.STUDIO_STARTER);
				}
			}
			if (difficulty.ButcherGangStatus == ButcherGangStatus.Butcher)
			{
				AchievementChecker.GetAchievement(AchievementName.A_BUTCHERED_DECISION);
			}
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Life <= 0)
			{
				AchievementChecker.GetAchievement(AchievementName.INK_MASTER);
			}
			GameManager.Instance.GameData.SetCredits(has: true);
			break;
		}
		}
		return result;
	}

	private static void GetAchievement(AchievementName achievementName)
	{
		GameManager.Instance.AchievementManager.SetAchievement(achievementName);
	}
}
