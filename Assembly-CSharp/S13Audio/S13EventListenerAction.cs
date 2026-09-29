using S13Audio.Malee;

namespace S13Audio;

public class S13EventListenerAction : S13EventListener
{
	[Reorderable]
	public S13LocalActionList onTriggerActions;

	protected override void OnTrigger()
	{
		if (this.onTriggerActions.IsExecutable)
		{
			S13Debug.Log("Heard Event " + base.listenTo.name + ", invoking " + base.name, this, S13Debug.LogType.Event);
			this.onTriggerActions.Execute();
		}
	}
}
