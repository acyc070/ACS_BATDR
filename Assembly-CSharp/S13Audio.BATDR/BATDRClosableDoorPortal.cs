using UnityEngine;

namespace S13Audio.BATDR;

[DefaultExecutionOrder(1100)]
public class BATDRClosableDoorPortal : S13ClosablePortal
{
	[SerializeField]
	private DoorID m_DoorID;

	private S13ClosablePortal m_ClosablePortal;

	private Door m_Door;

	private void Start()
	{
		if (this.m_DoorID != DoorID.NONE)
		{
			Door[] componentsInChildren;
			componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
			if (componentsInChildren != null)
			{
				foreach (Door door in componentsInChildren)
				{
					if (door.ID == this.m_DoorID)
					{
						this.m_ClosablePortal = door.S13CloseablePortal;
						this.m_Door = door;
						break;
					}
				}
			}
		}
		if (!this.m_ClosablePortal)
		{
			S13Debug.LogWarning("BATDRClosableDoorPortal on " + base.name + " is not set up, removing", base.gameObject);
			Object.Destroy(this);
		}
	}

	public override bool IsOpen()
	{
		if (this.m_ClosablePortal != null)
		{
			return this.m_ClosablePortal.IsOpen();
		}
		if (this.m_Door != null)
		{
			if (this.m_Door.Data != null)
			{
				if (this.m_Door.Data.Status != DoorStatus.Open)
				{
					return this.m_Door.Data.Status == DoorStatus.Open_Disabled;
				}
				return true;
			}
			if (this.m_Door.Content != null)
			{
				return this.m_Door.Content.IsActivated;
			}
		}
		return true;
	}
}
