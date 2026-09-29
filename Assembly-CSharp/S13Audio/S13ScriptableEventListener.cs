using S13Audio.Malee;
using UnityEngine;
using UnityEngine.Events;

namespace S13Audio;

[AddComponentMenu("")]
public class S13ScriptableEventListener : MonoBehaviour
{
	[SerializeField]
	private S13ScriptableEvent listenTo;

	[Header("Unity Event")]
	public UnityEvent onTrigger;

	[Header("Local Action List")]
	[Reorderable]
	public S13LocalActionList onTriggerActions;

	private void OnEnable()
	{
		S13Debug.LogWarning("S13ScriptableEventListener is depricated and should be replaced with S13EventListenerAction", this);
		if ((bool)this.listenTo)
		{
			this.listenTo.onTrigger += this.OnTrigger;
		}
	}

	private void OnDisable()
	{
		S13Debug.LogWarning("S13ScriptableEventListener is depricated and should be replaced with S13EventListenerAction", this);
		if ((bool)this.listenTo)
		{
			this.listenTo.onTrigger -= this.OnTrigger;
		}
	}

	private void OnTrigger()
	{
		S13Debug.Log("Heard Event " + this.listenTo.name + ", invoking " + base.name, this, S13Debug.LogType.Event);
		this.onTrigger?.Invoke();
		this.onTriggerActions.Execute();
	}
}
