public static class GentUpgradesRequirements
{
	public static int GetBatteryCasings(WeaponType weaponType)
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 1;
				break;
			case WeaponType.LEVEL_3:
				result = 1;
				break;
			}
			break;
		case DifficultyLevel.Normal:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 1;
				break;
			case WeaponType.LEVEL_3:
				result = 2;
				break;
			}
			break;
		case DifficultyLevel.Hard:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 1;
				break;
			case WeaponType.LEVEL_3:
				result = 3;
				break;
			}
			break;
		case DifficultyLevel.Impossible:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 1;
				break;
			case WeaponType.LEVEL_3:
				result = 6;
				break;
			}
			break;
		}
		return result;
	}

	public static int GetBatteries(WeaponType weaponType)
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			switch (weaponType)
			{
			case WeaponType.LEVEL_2:
				result = 4;
				break;
			case WeaponType.LEVEL_3:
				result = 10;
				break;
			}
			break;
		case DifficultyLevel.Normal:
			switch (weaponType)
			{
			case WeaponType.LEVEL_2:
				result = 6;
				break;
			case WeaponType.LEVEL_3:
				result = 12;
				break;
			}
			break;
		case DifficultyLevel.Hard:
			switch (weaponType)
			{
			case WeaponType.LEVEL_2:
				result = 8;
				break;
			case WeaponType.LEVEL_3:
				result = 14;
				break;
			}
			break;
		case DifficultyLevel.Impossible:
			switch (weaponType)
			{
			case WeaponType.LEVEL_2:
				result = 10;
				break;
			case WeaponType.LEVEL_3:
				result = 20;
				break;
			}
			break;
		}
		return result;
	}

	public static int GetToolkits(WeaponType weaponType)
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 3;
				break;
			case WeaponType.LEVEL_2:
				result = 10;
				break;
			case WeaponType.LEVEL_3:
				result = 10;
				break;
			}
			break;
		case DifficultyLevel.Normal:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 3;
				break;
			case WeaponType.LEVEL_2:
				result = 12;
				break;
			case WeaponType.LEVEL_3:
				result = 12;
				break;
			}
			break;
		case DifficultyLevel.Hard:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 3;
				break;
			case WeaponType.LEVEL_2:
				result = 14;
				break;
			case WeaponType.LEVEL_3:
				result = 14;
				break;
			}
			break;
		case DifficultyLevel.Impossible:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 3;
				break;
			case WeaponType.LEVEL_2:
				result = 20;
				break;
			case WeaponType.LEVEL_3:
				result = 20;
				break;
			}
			break;
		}
		return result;
	}

	public static int GetParts(WeaponType weaponType)
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 10;
				break;
			case WeaponType.LEVEL_2:
				result = 30;
				break;
			case WeaponType.LEVEL_3:
				result = 35;
				break;
			}
			break;
		case DifficultyLevel.Normal:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 10;
				break;
			case WeaponType.LEVEL_2:
				result = 35;
				break;
			case WeaponType.LEVEL_3:
				result = 40;
				break;
			}
			break;
		case DifficultyLevel.Hard:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 10;
				break;
			case WeaponType.LEVEL_2:
				result = 40;
				break;
			case WeaponType.LEVEL_3:
				result = 45;
				break;
			}
			break;
		case DifficultyLevel.Impossible:
			switch (weaponType)
			{
			case WeaponType.LEVEL_1:
				result = 10;
				break;
			case WeaponType.LEVEL_2:
				result = 50;
				break;
			case WeaponType.LEVEL_3:
				result = 55;
				break;
			}
			break;
		}
		return result;
	}
}
