using Steamworks;
using UnityEngine;

public class SteamAchievements : IAchievements
{
	private bool _IsInitialized;

	public void Initialize(AchievementIDMapping[] mapping = null)
	{
		this._IsInitialized = true;
	}

	public bool GetAchievement(AchievementName name)
	{
		SteamUserStats.RequestCurrentStats();
		SteamUserStats.GetAchievement(name.ToString(), out var pbAchieved);
		Debug.Log("Game ID: " + 1063660 + "\nAchievement: " + name.ToString() + "\nSteam Has Achievement: " + SteamUserStats.GetAchievement(name.ToString(), out pbAchieved) + "\nIs Achieved: " + pbAchieved);
		return pbAchieved;
	}

	public void SetAchievement(AchievementName name)
	{
		if (!this.GetAchievement(name))
		{
			Debug.Log("Achievement Found: " + name);
			SteamUserStats.SetAchievement(name.ToString());
			SteamUserStats.StoreStats();
		}
	}

	public void ClearAchievement(AchievementName name)
	{
		SteamUserStats.ClearAchievement(name.ToString());
	}

	public bool IsConnected()
	{
		if (GameManager.Instance.AchievementManager != null && this._IsInitialized)
		{
			return SteamUser.BLoggedOn();
		}
		return false;
	}
}
