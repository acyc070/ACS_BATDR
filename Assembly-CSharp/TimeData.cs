using System;
using UnityEngine;

[Serializable]
public class TimeData
{
	[SerializeField]
	private long m_Date;

	[SerializeField]
	private double m_TimePlayed;

	public long Date => this.m_Date;

	public double TimePlayed => this.m_TimePlayed;

	public TimeData()
	{
		this.m_Date = DateTime.Now.Ticks;
		this.m_TimePlayed = 0.0;
	}

	public TimeData(TimeData copy)
	{
		this.m_Date = copy.Date;
		this.m_TimePlayed = copy.TimePlayed;
	}

	public void Update()
	{
		this.m_TimePlayed += (DateTime.Now - new DateTime(this.m_Date)).TotalSeconds;
		this.m_Date = DateTime.Now.Ticks;
	}
}
