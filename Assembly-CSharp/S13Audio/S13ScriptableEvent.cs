using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/Event", order = 1)]
public class S13ScriptableEvent : ScriptableObject
{
	private static HashSet<S13ScriptableEvent> __referenceHolder;

	public event Action onTrigger;

	protected void OnEnable()
	{
		S13ScriptableEvent.__referenceHolder.Add(this);
	}

	[Obsolete("Method S13ScriptableEvent::Execute() has been deprecated. Use S13ScriptableEvent::Raise()")]
	public void Execute()
	{
		S13Debug.LogWarning("Method S13ScriptableEvent::Execute() has been deprecated. Use S13ScriptableEvent::Raise(). Raise() will now be called");
		this.Raise();
	}

	public void Raise()
	{
		S13Debug.Log("Event " + base.name + " raised", this, S13Debug.LogType.Event);
		this.onTrigger?.Invoke();
	}

	static S13ScriptableEvent()
	{
		S13ScriptableEvent.__referenceHolder = new HashSet<S13ScriptableEvent>();
	}
}
