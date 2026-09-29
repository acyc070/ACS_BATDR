using UnityEngine;

namespace S13Audio;

public class S13ActionTrigger : S13ConditionalTrigger
{
	[SerializeField]
	public S13LocalAction onEnterAction;

	[SerializeField]
	public S13LocalAction onExitAction;

	protected override void OnEnter(ConditionInfo colliderInfo)
	{
		S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " Entered by " + colliderInfo.name, base.gameObject, S13Debug.LogType.TriggerMessage);
		this.onEnterAction.Execute();
	}

	protected override void OnExit(ConditionInfo colliderInfo)
	{
		S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " Exited by " + colliderInfo.name, base.gameObject, S13Debug.LogType.TriggerMessage);
		this.onExitAction.Execute();
	}
}
