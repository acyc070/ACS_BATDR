using System;
using UnityEngine;

public class ObjectiveRequirements : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectiveRequirements :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
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
			base.SendOnComplete();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveRequirements :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("ObjectiveRequirements :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		base.SendOnComplete();
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
