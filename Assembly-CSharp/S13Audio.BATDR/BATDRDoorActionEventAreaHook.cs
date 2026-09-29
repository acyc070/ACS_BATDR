using System;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDoorActionEventAreaHook : S13ConditionalTrigger
{
	[SerializeField]
	private DoorID m_DoorID;

	[SerializeField]
	private S13LocalAction onActivateAction;

	[SerializeField]
	private S13LocalAction onActivatedAction;

	[SerializeField]
	private S13LocalAction onDeactivateAction;

	[SerializeField]
	private S13LocalAction onDeactivatedAction;

	private ActionEventController targetController;

	protected override void Start()
	{
		base.Start();
		if (this.m_DoorID == DoorID.NONE)
		{
			return;
		}
		Door[] componentsInChildren;
		componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		if (componentsInChildren == null)
		{
			return;
		}
		foreach (Door door in componentsInChildren)
		{
			if (door.ID == this.m_DoorID)
			{
				this.targetController = door;
				break;
			}
		}
	}

	protected override void OnEnter(ConditionInfo colliderInfo)
	{
		if (!(this.targetController == null))
		{
			this.targetController.OnActivate += this.OnActivate;
			this.targetController.OnActivate += this.OnActivated;
			this.targetController.OnDeactivate += this.OnDeactivate;
			this.targetController.OnDeactivate += this.OnDeactivated;
		}
	}

	protected override void OnExit(ConditionInfo colliderInfo)
	{
		if (!(this.targetController == null))
		{
			this.targetController.OnActivate -= this.OnActivate;
			this.targetController.OnActivate -= this.OnActivated;
			this.targetController.OnDeactivate -= this.OnDeactivate;
			this.targetController.OnDeactivate -= this.OnDeactivated;
		}
	}

	private void OnActivate(object obj, EventArgs args)
	{
		if (this.onActivateAction.IsExecutable)
		{
			this.onActivateAction.Execute();
		}
	}

	private void OnActivated(object obj, EventArgs args)
	{
		if (this.onActivatedAction.IsExecutable)
		{
			this.onActivatedAction.Execute();
		}
	}

	private void OnDeactivate(object obj, EventArgs args)
	{
		if (this.onDeactivateAction.IsExecutable)
		{
			this.onDeactivateAction.Execute();
		}
	}

	private void OnDeactivated(object obj, EventArgs args)
	{
		if (this.onDeactivatedAction.IsExecutable)
		{
			this.onDeactivatedAction.Execute();
		}
	}
}
