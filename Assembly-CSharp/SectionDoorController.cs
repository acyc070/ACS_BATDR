using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionDoorController : SectionController
{
	private Door[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Door>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			Door controller;
			controller = this.m_Group[i];
			if (!(controller != null))
			{
				continue;
			}
			this.RemoveListeners(ref controller);
			if (!controller.InitializeOnAwake)
			{
				controller.Initialize();
			}
			DoorDataObject doorDataObject;
			doorDataObject = (DoorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<DoorID, DoorDataObject>(controller.SectionID, controller.ID);
			if (doorDataObject != null)
			{
				controller.Data.SetStatus(doorDataObject.Status);
				controller.Data.SetType(doorDataObject.Type);
				if (doorDataObject.Status == DoorStatus.Open)
				{
					controller.Content.ForceActivateComplete();
				}
				else if (doorDataObject.Status == DoorStatus.Open_Disabled)
				{
					controller.Content.ForceActivateComplete();
					controller.Content.Disable();
				}
				else if (doorDataObject.Status == DoorStatus.Closed)
				{
					controller.Content.ForceDeactivateComplete();
				}
				else if (doorDataObject.Status == DoorStatus.Closed_Locked)
				{
					controller.Content.ForceInactive();
				}
				else if (doorDataObject.Status == DoorStatus.Closed_Disabled)
				{
					controller.Content.ForceDeactivateComplete();
					controller.Content.Disable();
				}
			}
			else
			{
				doorDataObject = DataObject<DoorID, DoorDataObject>.Create(controller.ID);
				doorDataObject.SetStatus(controller.Data.Status);
				doorDataObject.SetType(controller.Data.Type);
				GameManager.Instance.GameData.CurrentSave.AddData(controller.SectionID, doorDataObject);
				if (doorDataObject.Status == DoorStatus.Open)
				{
					controller.Content.ForceActivateComplete();
				}
				else if (doorDataObject.Status == DoorStatus.Open_Disabled)
				{
					controller.Content.ForceActivateComplete();
					controller.Content.Disable();
				}
				else if (controller.Data.Status == DoorStatus.Closed_Locked)
				{
					controller.Content.ForceInactive();
				}
				else if (controller.Data.Status == DoorStatus.Closed_Disabled)
				{
					controller.Content.ForceDeactivateComplete();
					controller.Content.Disable();
				}
			}
			this.AddListeners(ref controller);
		}
		yield return null;
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Door door;
		door = sender as Door;
		DoorStatus status;
		status = DoorStatus.Open;
		if (door.Content.IsInactive)
		{
			status = DoorStatus.Open_Disabled;
		}
		this.SetDoorStatus(door, status);
	}

	private void HandleControllerOnDeactivate(object sender, EventArgs e)
	{
		Door door;
		door = sender as Door;
		DoorStatus status;
		status = DoorStatus.Closed;
		if (door.Content.IsInactive)
		{
			status = ((!door.Content.IsDisabled) ? DoorStatus.Closed_Locked : DoorStatus.Closed_Disabled);
		}
		this.SetDoorStatus(door, status);
	}

	private void SetDoorStatus(Door door, DoorStatus status)
	{
		door.Data.SetStatus(status);
		DoorDataObject doorDataObject;
		doorDataObject = (DoorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<DoorID, DoorDataObject>(door.SectionID, door.ID);
		if (doorDataObject != null)
		{
			doorDataObject.SetStatus(door.Data.Status);
			doorDataObject.SetType(door.Data.Type);
			return;
		}
		doorDataObject = DataObject<DoorID, DoorDataObject>.Create(door.ID);
		doorDataObject.SetStatus(door.Data.Status);
		doorDataObject.SetType(door.Data.Type);
		GameManager.Instance.GameData.CurrentSave.AddData(door.SectionID, doorDataObject);
	}

	private void AddListeners(ref Door controller)
	{
		controller.OnActivate += this.HandleControllerOnActivate;
		controller.OnDeactivate += this.HandleControllerOnDeactivate;
	}

	private void RemoveListeners(ref Door controller)
	{
		controller.OnActivate -= this.HandleControllerOnActivate;
		controller.OnDeactivate -= this.HandleControllerOnDeactivate;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				Door controller;
				controller = this.m_Group[i];
				this.RemoveListeners(ref controller);
			}
		}
	}
}
