using UnityEngine;

public class ObjectiveChapterTitle : Objective
{
	[SerializeField]
	private ChapterID m_ChapterID;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		string chapter;
		chapter = TextUtility.GetKey(Chapter.GetChapterString(this.m_ChapterID)).ToUpper() + ":";
		string title;
		title = TextUtility.GetKey(Chapter.GetChapterName(this.m_ChapterID)).ToUpper();
		GameManager.Instance.ShowChapterTitle(chapter, title);
		this.CheckAchievement();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		base.SendOnComplete();
	}

	private void CheckAchievement()
	{
		if (this.m_ChapterID == ChapterID.Chapter_Five)
		{
			AchievementChecker.Check(AchievementName.TIMELESS_REMAINS);
		}
		else if (this.m_ChapterID == ChapterID.Chapter_Four)
		{
			AchievementChecker.Check(AchievementName.THRILLS_AND_SPILLS);
		}
		else if (this.m_ChapterID == ChapterID.Chapter_Three)
		{
			AchievementChecker.Check(AchievementName.RUBBERHOSE_NIGHTMARE);
		}
		else if (this.m_ChapterID == ChapterID.Chapter_Two)
		{
			AchievementChecker.Check(AchievementName.CARTOON_MADNESS);
		}
		else if (this.m_ChapterID == ChapterID.Chapter_One)
		{
			AchievementChecker.Check(AchievementName.WELCOME_TO_THE_STUDIO);
		}
	}
}
