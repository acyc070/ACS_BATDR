using UnityEngine;

public static class TimerCheck
{
	public static float InkDemon()
	{
		return Random.Range(TimerCheck.InkDemonTinkerMin(), TimerCheck.InkDemonTimerMax());
	}

	private static float InkDemonTinkerMin()
	{
		float result;
		result = 180f;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 160f;
			break;
		case DifficultyLevel.Hard:
			result = 140f;
			break;
		case DifficultyLevel.Impossible:
			result = 100f;
			break;
		}
		return result;
	}

	private static float InkDemonTimerMax()
	{
		float result;
		result = 300f;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 280f;
			break;
		case DifficultyLevel.Hard:
			result = 260f;
			break;
		case DifficultyLevel.Impossible:
			result = 200f;
			break;
		}
		return result;
	}
}
