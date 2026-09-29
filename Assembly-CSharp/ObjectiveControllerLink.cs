using System;
using UnityEngine;

public class ObjectiveControllerLink : Objective
{
	[SerializeField]
	private ObjectiveController m_ObjectiveController;

	protected override void InternalInitialize()
	{
		if (!(this.m_ObjectiveController == null))
		{
			if (this.m_ObjectiveController.IsComplete)
			{
				base.ForceComplete();
			}
			else
			{
				this.m_ObjectiveController.OnComplete += this.HandleObjectiveControllerOnComplete;
			}
		}
	}

	protected override void InternalForceComplete()
	{
		if (!(this.m_ObjectiveController == null))
		{
			this.m_ObjectiveController.ForceComplete();
			base.SendOnComplete();
		}
	}

	protected override void InternalInactive()
	{
		if (!(this.m_ObjectiveController == null))
		{
			this.m_ObjectiveController.Complete();
			base.SendOnComplete();
		}
	}

	private void HandleObjectiveControllerOnComplete(object sender, EventArgs e)
	{
		this.m_ObjectiveController.OnComplete -= this.HandleObjectiveControllerOnComplete;
		base.SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (this.m_ObjectiveController != null)
		{
			this.m_ObjectiveController.OnComplete -= this.HandleObjectiveControllerOnComplete;
		}
		base.OnDisposed();
	}
}
