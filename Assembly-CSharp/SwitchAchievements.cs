using UnityEngine;

public class SwitchAchievements : IAchievements
{
	public void Initialize(AchievementIDMapping[] mapping = null)
	{
	}

	public bool GetAchievement(AchievementName name)
	{
		Debug.Log("*** SwitchAchievements = in GetAchievement()");
		return false;
	}

	public void SetAchievement(AchievementName name)
	{
	}

	public void ClearAchievement(AchievementName name)
	{
	}

	public bool IsConnected()
	{
		return false;
	}

	private void SendUpdateStats()
	{
	}
}
