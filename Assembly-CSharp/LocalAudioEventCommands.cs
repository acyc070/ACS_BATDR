using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalAudioEventCommands : JMonoBehaviour
{
	[SerializeField]
	public MonoBehaviour m_Sender;

	[SerializeField]
	public List<LocalAudioCommand> m_EventCallbacks = new List<LocalAudioCommand>();

	private Dictionary<Delegate, string> m_Delegates;

	public override void OnEnable()
	{
		if (this.m_Sender != null)
		{
			this.m_Delegates = new Dictionary<Delegate, string>();
			for (int i = 0; i < this.m_EventCallbacks.Count; i = unchecked(i + 1))
			{
				LocalAudioCommand localAudioCommand;
				localAudioCommand = this.m_EventCallbacks[i];
				Delegate key;
				key = DelegateUtility.AddEventHandler(this.m_Sender, localAudioCommand.EventHandler, localAudioCommand.Execute);
				this.m_Delegates.Add(key, localAudioCommand.EventHandler);
			}
		}
	}

	public override void OnDisable()
	{
		this.ClearEventHandlers();
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
