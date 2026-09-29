using System;
using UnityEngine;
using UnityEngine.Events;

namespace S13Audio;

[Serializable]
public class S13Action_UnityEvent : IS13Executable
{
	[Tooltip("The Unity event to execute.")]
	[SerializeField]
	public UnityEvent unityEvent;

	public bool IsExecutable => this.unityEvent.GetPersistentEventCount() > 0;

	public void Execute()
	{
		this.unityEvent.Invoke();
	}
}
