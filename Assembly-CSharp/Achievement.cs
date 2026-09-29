public class Achievement
{
	public int ID;

	public AchievementName AchievementName { get; private set; }

	public string DisplayName { get; private set; }

	public Achievement(int id, AchievementName name, string displayName)
	{
		this.ID = id;
		this.AchievementName = name;
		this.DisplayName = displayName;
	}

	public Achievement()
	{
	}
}
