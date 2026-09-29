using System;
using UnityEngine;

public class ObjectiveGentLock : Objective
{
	[Header("Gent Lock")]
	[SerializeField]
	private GentLock m_GentLock;

	protected override void InternalInitialize()
	{
		this.m_GentLock.OnComplete -= this.HandleCutsceneOnComplete;
		this.m_GentLock.OnComplete += this.HandleCutsceneOnComplete;
		if (this.m_GentLock.Content != null && this.m_GentLock.Content.Activator != null && this.m_GentLock.Content.Activator.ActivateType == CutsceneActivateType.Callback)
		{
			this.m_GentLock.Play();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		this.m_GentLock.OnComplete -= this.HandleCutsceneOnComplete;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.RemoveListeners();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (this.m_GentLock != null)
		{
			this.m_GentLock.OnComplete -= this.HandleCutsceneOnComplete;
		}
	}
}
