using UnityEngine;
using UnityEngine.Events;

namespace S13Audio;

public class S13EventListenerUnityEvent : S13EventListener
{
	[Header("Unity Event")]
	public UnityEvent onTrigger;

	protected override void OnTrigger()
	{
		S13Debug.Log("Heard Event " + base.listenTo.name + ", invoking " + base.name, this, S13Debug.LogType.Event);
		this.onTrigger?.Invoke();
	}
}
