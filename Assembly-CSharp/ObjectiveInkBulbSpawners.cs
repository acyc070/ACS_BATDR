using UnityEngine;

public class ObjectiveInkBulbSpawners : Objective
{
	[SerializeField]
	private InkBulb[] m_InkBulbs;

	[SerializeField]
	private bool m_EnableOnForceComplete;

	protected override void InternalInitialize()
	{
		this.EnableInkBulbs();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (this.m_EnableOnForceComplete)
		{
			this.EnableInkBulbs();
		}
		base.SendOnComplete();
	}

	private void EnableInkBulbs()
	{
		this.m_InkBulbs.Shuffle();
		for (int i = 0; i < this.GetMaxBulbs(); i = unchecked(i + 1))
		{
			InkBulb inkBulb;
			inkBulb = this.m_InkBulbs[i];
			inkBulb.SetActive(active: true);
			if (this.m_EnableOnForceComplete)
			{
				inkBulb.Activate();
			}
		}
	}

	private int GetMaxBulbs()
	{
		int num;
		num = this.m_InkBulbs.Length;
		unchecked
		{
			switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
			{
			case DifficultyLevel.Easy:
				num /= 4;
				break;
			case DifficultyLevel.Normal:
				num /= 2;
				break;
			}
			return num;
		}
	}
}
