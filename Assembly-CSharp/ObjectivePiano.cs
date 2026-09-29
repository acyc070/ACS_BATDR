using System;
using UnityEngine;

public class ObjectivePiano : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Piano")]
	[SerializeField]
	private Piano m_Piano;

	[SerializeField]
	private bool m_IsSuccess;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void HandlePiano(object sender, EventArgs e)
	{
		base.SendOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
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
			if (this.m_IsSuccess)
			{
				this.m_Piano.OnSuccess += this.HandlePiano;
			}
			else
			{
				this.m_Piano.OnFail += this.HandlePiano;
			}
			this.m_Piano.Initialize();
		}
		return flag;
	}

	protected override void InternalForceComplete()
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.m_Piano.OnSuccess -= this.HandlePiano;
		this.m_Piano.OnFail -= this.HandlePiano;
	}
}
