public class AchievementManager : JDisposable
{
	private IAchievements m_AchievementController;

	private AchievementIDMapping[] m_AchievementIDs;

	public static AchievementManager Create()
	{
		return new AchievementManager();
	}

	protected AchievementManager()
	{
	}

	public void Init()
	{
		this.m_AchievementIDs = new AchievementIDMapping[0];
		if (this.m_AchievementController != null)
		{
			this.m_AchievementController.Initialize(this.m_AchievementIDs);
		}
	}

	public bool GetAchievement(AchievementName key)
	{
		if (!this.CanUseAchievements())
		{
			return false;
		}
		return this.m_AchievementController.GetAchievement(key);
	}

	public void SetAchievement(AchievementName key)
	{
		if (this.CanUseAchievements())
		{
			this.m_AchievementController.SetAchievement(key);
		}
	}

	public void ClearAchievement(AchievementName key)
	{
		if (this.CanUseAchievements())
		{
			this.m_AchievementController.ClearAchievement(key);
		}
	}

	private bool CanUseAchievements()
	{
		if (this.m_AchievementController != null)
		{
			return this.m_AchievementController.IsConnected();
		}
		return false;
	}

	protected override void OnDisposed()
	{
		this.m_AchievementController = null;
		this.m_AchievementIDs = null;
		base.OnDisposed();
	}
}
