using UnityEngine;

namespace S13Audio;

[DefaultExecutionOrder(-10)]
public abstract class S13EventListener : MonoBehaviour
{
	[SerializeField]
	protected S13ScriptableEvent listenTo;

	private void OnEnable()
	{
		if ((bool)this.listenTo)
		{
			this.listenTo.onTrigger += this.OnTrigger;
		}
	}

	private void OnDisable()
	{
		if ((bool)this.listenTo)
		{
			this.listenTo.onTrigger -= this.OnTrigger;
		}
	}

	protected abstract void OnTrigger();
}
