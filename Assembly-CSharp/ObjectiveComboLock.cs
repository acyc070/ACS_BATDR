using System;
using UnityEngine;

public class ObjectiveComboLock : Objective
{
	[SerializeField]
	private ComboLock m_ComboLock;

	protected override void InternalInitialize()
	{
		if (!this.m_ComboLock.IsComplete)
		{
			this.m_ComboLock.OnComplete += this.HandleComboLockOnComplete;
		}
	}

	private void HandleComboLockOnComplete(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		this.m_ComboLock.OnComplete -= this.HandleComboLockOnComplete;
	}
}
