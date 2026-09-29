using System;
using UnityEngine;

public class ObjectiveDoor : Objective
{
	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private bool m_ForceComplete;

	[SerializeField]
	private bool m_EnabledOnComplete = true;

	[SerializeField]
	private bool m_InitializeDisables;

	[SerializeField]
	private bool m_CheckInactive;

	[SerializeField]
	private DoorType m_DoorOpenType;

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		this.CheckEnableOnComplete();
		if (!this.m_InitializeDisables)
		{
			if (this.m_CheckInactive)
			{
				if (!this.m_Door.Content.IsActivated || this.m_Door.Content.IsPlaying)
				{
					this.m_Door.Content.ForceActivate();
				}
			}
			else
			{
				this.m_Door.Content.ForceActivate();
			}
		}
		else if (this.m_CheckInactive)
		{
			if (this.m_Door.Content.IsActivated || this.m_Door.Content.IsPlaying)
			{
				this.m_Door.Content.ForceDeactivate();
			}
		}
		else
		{
			this.m_Door.Content.ForceDeactivate();
		}
		base.SendOnComplete();
	}

	protected override void InternalEnable()
	{
		this.RemoveListeners();
		this.m_Door.OnActivated += this.HandleDoorOnActivated;
		if (this.m_Door.Data.Status == DoorStatus.Closed || this.m_Door.Data.Status == DoorStatus.Open)
		{
			this.m_Door.Content.Enable();
		}
	}

	protected override void InternalInactive()
	{
		this.RemoveListeners();
		this.m_Door.OnInactive += this.HandleActionDoorOnInactive;
		this.m_Door.Content.ForceInactive();
	}

	protected override void InternalDisable()
	{
		this.RemoveListeners();
		this.m_Door.Content.Disable();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected override void InternalForceComplete()
	{
		if (this.m_ForceComplete)
		{
			if (!this.m_InitializeDisables)
			{
				if (this.m_DoorOpenType != DoorType.Single)
				{
					this.m_Door.Data.SetType(this.m_DoorOpenType);
				}
				if (this.m_Door.Content.IsActivated)
				{
					this.m_Door.Content.ForceDeactivateComplete();
				}
				if (!this.m_Door.Content.IsActivated)
				{
					this.m_Door.Content.ForceActivateComplete();
				}
			}
			else if (this.m_Door.Content.IsActivated)
			{
				this.m_Door.Content.ForceDeactivateComplete();
			}
		}
		this.CheckEnableOnComplete();
		base.SendOnComplete();
	}

	private void CheckEnableOnComplete()
	{
		if (this.m_EnabledOnComplete)
		{
			this.m_Door.Content.Enable();
		}
		else
		{
			this.m_Door.Content.Disable();
		}
	}

	private void HandleDoorOnActivated(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	private void HandleActionDoorOnInactive(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (this.m_Door != null)
		{
			this.m_Door.OnActivate -= this.HandleDoorOnActivated;
			this.m_Door.OnActivated -= this.HandleDoorOnActivated;
			this.m_Door.OnInactive -= this.HandleActionDoorOnInactive;
		}
	}
}
