using System;
using UnityEngine;

public class ObjectiveHardlyWorking : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Settings")]
	[SerializeField]
	private float m_Length = 900f;

	private float m_Time;

	private bool m_IsActivated;

	protected override void InternalUpdate()
	{
		if (this.m_IsActivated)
		{
			if (this.m_Time >= this.m_Length)
			{
				this.Unlock();
			}
			else
			{
				this.m_Time += Time.deltaTime;
			}
		}
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void Unlock()
	{
		GameManager.Instance.LockPause();
		this.Deactivate();
		base.SendOnComplete();
	}

	protected override void InternalDisable()
	{
		this.Deactivate();
	}

	protected override void InternalComplete()
	{
		this.Deactivate();
	}

	protected override void InternalForceComplete()
	{
		this.Deactivate();
	}

	private void Deactivate()
	{
		this.m_IsActivated = false;
		this.m_Time = 0f;
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			if (!this.m_IsActivated)
			{
				this.m_IsActivated = true;
				this.m_Time = 0f;
			}
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}
}
