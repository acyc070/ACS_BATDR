using System;
using UnityEngine;

public class ObjectiveGentPower : Objective
{
	[Header("Gent Power")]
	[SerializeField]
	private GentPower m_GentPower;

	protected override void InternalInitialize()
	{
		JDebug.Log("GentPower :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		this.m_GentPower.OnComplete -= this.HandleCutsceneOnComplete;
		this.m_GentPower.OnComplete += this.HandleCutsceneOnComplete;
		if (this.m_GentPower.Content.Activator.ActivateType == CutsceneActivateType.Callback)
		{
			this.m_GentPower.Play();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		JDebug.Log("GentPower :: HandleCutsceneOnComplete", this, JDebug.JDebugType.Objectives);
		this.m_GentPower.OnComplete -= this.HandleCutsceneOnComplete;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("GentPower :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		JDebug.Log("GentPower :: InternalComplete", this, JDebug.JDebugType.Objectives);
		this.RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (this.m_GentPower != null)
		{
			this.m_GentPower.OnComplete -= this.HandleCutsceneOnComplete;
		}
	}
}
