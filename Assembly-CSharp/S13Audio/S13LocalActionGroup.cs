using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

public class S13LocalActionGroup : MonoBehaviour, IS13ActionGroup
{
	[SerializeField]
	[Reorderable]
	public S13LocalActionList actions;

	[SerializeField]
	private bool allowWhileDisabled;

	public void Execute()
	{
		if (!base.isActiveAndEnabled && !this.allowWhileDisabled)
		{
			S13Debug.Log("Failed to execute Local action group " + base.name + ". Script is not enabled or GameObject is inactive in hierarchy", this, S13Debug.LogType.Action);
			return;
		}
		S13Debug.Log("Executing Local action group " + base.name, this, S13Debug.LogType.Action);
		foreach (S13LocalAction action in this.actions)
		{
			action.Execute();
		}
	}

	public override string ToString()
	{
		return "Local Action Group " + base.ToString();
	}
}
