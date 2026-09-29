using System;
using UnityEngine;

public class ObjectiveGentRecharger : Objective
{
	[Header("Gent Recharger")]
	[SerializeField]
	private GentRecharger m_GentRecharger;

	[SerializeField]
	private bool m_IsBattery;

	[SerializeField]
	private bool m_IsDisable;

	protected override void InternalEnable()
	{
		if (this.m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalEnable", this, JDebug.JDebugType.Objectives);
			this.m_GentRecharger.SetGentRecharger(active: true);
		}
	}

	protected override void InternalDisable()
	{
		if (this.m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalDisable", this, JDebug.JDebugType.Objectives);
			this.m_GentRecharger.SetGentRecharger(active: false);
		}
	}

	protected override void InternalInitialize()
	{
		if (!this.m_IsDisable)
		{
			JDebug.Log("GentRecharger :: InternalInitialize", this, JDebug.JDebugType.Objectives);
			if (!this.m_IsBattery)
			{
				this.m_GentRecharger.InteractableCharger.OnInteractionExit -= this.HandleGentRechargerOnInteractionExit;
				this.m_GentRecharger.InteractableCharger.OnInteractionExit += this.HandleGentRechargerOnInteractionExit;
			}
			else
			{
				this.m_GentRecharger.InteractableBatteries.OnInteract -= this.HandleGentRechargerBatteryOnInteract;
				this.m_GentRecharger.InteractableBatteries.OnInteract += this.HandleGentRechargerBatteryOnInteract;
			}
		}
	}

	private void HandleGentRechargerOnInteractionExit(object sender, EventArgs e)
	{
		JDebug.Log("GentRecharger :: HandleGentRechargerOnInteractionExit", this, JDebug.JDebugType.Objectives);
		this.m_GentRecharger.InteractableCharger.OnInteractionExit -= this.HandleGentRechargerOnInteractionExit;
		base.SendOnComplete();
	}

	private void HandleGentRechargerBatteryOnInteract(object sender, EventArgs e)
	{
		JDebug.Log("GentRecharger :: HandleGentRechargerOnInteractionExit", this, JDebug.JDebugType.Objectives);
		this.m_GentRecharger.InteractableBatteries.OnInteract -= this.HandleGentRechargerBatteryOnInteract;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("GentRecharger :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		if (this.m_IsDisable)
		{
			this.m_GentRecharger.SetGentRecharger(active: true);
		}
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		JDebug.Log("GentRecharger :: InternalComplete", this, JDebug.JDebugType.Objectives);
		this.RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (this.m_GentRecharger != null)
		{
			this.m_GentRecharger.InteractableCharger.OnInteractionExit -= this.HandleGentRechargerOnInteractionExit;
			this.m_GentRecharger.InteractableBatteries.OnInteract -= this.HandleGentRechargerBatteryOnInteract;
		}
	}
}
