using UnityEngine;

public class ObjectiveAchievement : Objective
{
	[SerializeField]
	private AchievementName m_AchievementName;

	protected override void InternalInitialize()
	{
		AchievementChecker.Check(this.m_AchievementName);
		base.SendOnComplete();
	}
}
