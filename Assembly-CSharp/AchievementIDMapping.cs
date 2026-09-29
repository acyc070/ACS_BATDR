using System;

[Serializable]
public class AchievementIDMapping
{
	public AchievementName AchievementName;

	public int AchievementID;

	public AchievementIDMapping(int id, AchievementName name)
	{
		this.AchievementID = id;
		this.AchievementName = name;
	}
}
