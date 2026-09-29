using System;
using UnityEngine;

[Serializable]
public class DifficultyData
{
	[SerializeField]
	private DifficultyLevel m_Difficulty;

	[SerializeField]
	private bool m_DifficultyChanged;

	[SerializeField]
	private ButcherGangStatus m_ButcherGangStatus;

	[SerializeField]
	private bool m_IsNotSocialite;

	public DifficultyLevel Difficulty => this.m_Difficulty;

	public bool DifficultyChanged => this.m_DifficultyChanged;

	public ButcherGangStatus ButcherGangStatus => this.m_ButcherGangStatus;

	public bool IsNotSocialite => this.m_IsNotSocialite;

	public DifficultyData(DifficultyLevel difficulty)
	{
		this.m_Difficulty = difficulty;
		this.m_DifficultyChanged = false;
	}

	public void Change(DifficultyLevel difficulty)
	{
		this.m_Difficulty = difficulty;
		this.m_DifficultyChanged = true;
	}

	public void SetButcherGangStatus(ButcherGangStatus status)
	{
		this.m_ButcherGangStatus = status;
	}

	public void UpdateSocialite()
	{
		this.m_IsNotSocialite = true;
	}
}
