public interface IAchievements
{
	void Initialize(AchievementIDMapping[] mapping = null);

	bool GetAchievement(AchievementName name);

	void SetAchievement(AchievementName name);

	void ClearAchievement(AchievementName name);

	bool IsConnected();
}
