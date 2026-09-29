using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDoorEventActions : JMonoBehaviour
{
	[SerializeField]
	public DoorID m_DoorID;

	[SerializeField]
	public List<LocalAudioCommand> m_EventCallbacks = new List<LocalAudioCommand>();

	[SerializeField]
	public bool m_EnableFallback;

	[SerializeField]
	public int m_FallbackID;

	private MonoBehaviour m_Sender;

	private Dictionary<Delegate, string> m_Delegates;

	public override void OnEnable()
	{
		this.m_Sender = this.GetSenderDoor();
		unchecked
		{
			if (this.m_Sender != null)
			{
				this.m_Delegates = new Dictionary<Delegate, string>();
				for (int i = 0; i < this.m_EventCallbacks.Count; i++)
				{
					LocalAudioCommand localAudioCommand;
					localAudioCommand = this.m_EventCallbacks[i];
					Delegate key;
					key = DelegateUtility.AddEventHandler(this.m_Sender, localAudioCommand.EventHandler, localAudioCommand.Execute);
					this.m_Delegates.Add(key, localAudioCommand.EventHandler);
				}
			}
			else
			{
				if (!this.m_EnableFallback)
				{
					return;
				}
				for (int j = 0; j < this.m_EventCallbacks.Count; j++)
				{
					LocalAudioCommand localAudioCommand2;
					localAudioCommand2 = this.m_EventCallbacks[j];
					if (localAudioCommand2.ID == this.m_FallbackID)
					{
						localAudioCommand2.Execute();
					}
				}
			}
		}
	}

	public override void OnDisable()
	{
		this.ClearEventHandlers();
	}

	public MonoBehaviour GetSenderDoor()
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
						return door;
					}
				}
			}
		}
		return null;
	}

	private void ClearEventHandlers()
	{
		if (this.m_Sender != null && this.m_Delegates != null)
		{
			foreach (Delegate key in this.m_Delegates.Keys)
			{
				DelegateUtility.RemoveEventHandler(this.m_Sender, this.m_Delegates[key], key);
			}
		}
		if (this.m_Delegates != null)
		{
			this.m_Delegates.Clear();
			this.m_Delegates = null;
		}
	}

	protected override void OnDisposed()
	{
		this.ClearEventHandlers();
		base.OnDisposed();
	}
}
