using System;
using UnityEngine;

public class ObjectiveMantle : Objective
{
	[SerializeField]
	private Mantle m_Mantle;

	protected override void InternalInitialize()
	{
		if (this.m_Mantle != null && this.m_Mantle.FlowMantle != null)
		{
			this.m_Mantle.FlowMantle.OnEnter -= this.HandleMantleOnEnter;
			this.m_Mantle.FlowMantle.OnEnter += this.HandleMantleOnEnter;
		}
	}

	private void HandleMantleOnEnter(object sender, EventArgs e)
	{
		base.SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (this.m_Mantle != null && this.m_Mantle.FlowMantle != null)
		{
			this.m_Mantle.FlowMantle.OnEnter -= this.HandleMantleOnEnter;
		}
		base.OnDisposed();
	}
}
