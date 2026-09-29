public static class UpgradeCheck
{
	public static float GetHealth()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth == 1)
		{
			return UpgradeCheck.CheckHealthDifficulty(1);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth == 2)
		{
			return UpgradeCheck.CheckHealthDifficulty(2);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth == 3)
		{
			return UpgradeCheck.CheckHealthDifficulty(3);
		}
		return UpgradeCheck.CheckHealthDifficulty(0);
	}

	private static float CheckHealthDifficulty(int level)
	{
		return GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty switch
		{
			DifficultyLevel.Normal => level switch
			{
				1 => 11f, 
				2 => 12f, 
				3 => 13f, 
				_ => 10f, 
			}, 
			DifficultyLevel.Hard => level switch
			{
				1 => 10f, 
				2 => 12f, 
				3 => 14f, 
				_ => 9f, 
			}, 
			DifficultyLevel.Impossible => level switch
			{
				1 => 7f, 
				2 => 8f, 
				3 => 10f, 
				_ => 5f, 
			}, 
			_ => level switch
			{
				1 => 12f, 
				2 => 14f, 
				3 => 15f, 
				_ => 10f, 
			}, 
		};
	}

	public static float GetStamina()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina == 1)
		{
			return UpgradeCheck.CheckStaminaDifficulty(1);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina == 2)
		{
			return UpgradeCheck.CheckStaminaDifficulty(2);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina == 3)
		{
			return UpgradeCheck.CheckStaminaDifficulty(3);
		}
		return UpgradeCheck.CheckStaminaDifficulty(0);
	}

	private static float CheckStaminaDifficulty(int level)
	{
		return GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty switch
		{
			DifficultyLevel.Normal => level switch
			{
				1 => 12f, 
				2 => 14f, 
				3 => 18f, 
				_ => 10f, 
			}, 
			DifficultyLevel.Hard => level switch
			{
				1 => 10f, 
				2 => 12.5f, 
				3 => 15f, 
				_ => 8f, 
			}, 
			DifficultyLevel.Impossible => level switch
			{
				1 => 8f, 
				2 => 10f, 
				3 => 12f, 
				_ => 5f, 
			}, 
			_ => level switch
			{
				1 => 12.5f, 
				2 => 15f, 
				3 => 20f, 
				_ => 10f, 
			}, 
		};
	}

	public static float GetAbility()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility == 1)
		{
			return UpgradeCheck.GetAbilityDifficulty(1);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility == 2)
		{
			return UpgradeCheck.GetAbilityDifficulty(2);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility == 3)
		{
			return UpgradeCheck.GetAbilityDifficulty(3);
		}
		return UpgradeCheck.GetAbilityDifficulty(0);
	}

	private static float GetAbilityDifficulty(int level)
	{
		return GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty switch
		{
			DifficultyLevel.Normal => level switch
			{
				1 => 8f, 
				2 => 6f, 
				3 => 3f, 
				_ => 10f, 
			}, 
			DifficultyLevel.Hard => level switch
			{
				1 => 9f, 
				2 => 6f, 
				3 => 4f, 
				_ => 11f, 
			}, 
			DifficultyLevel.Impossible => level switch
			{
				1 => 14f, 
				2 => 12f, 
				3 => 10f, 
				_ => 15f, 
			}, 
			_ => level switch
			{
				1 => 7.5f, 
				2 => 5f, 
				3 => 2.5f, 
				_ => 10f, 
			}, 
		};
	}
}
