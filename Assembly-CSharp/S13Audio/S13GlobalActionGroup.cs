using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/Action Group", order = 0)]
public class S13GlobalActionGroup : ScriptableObject, IS13ActionGroup
{
	[SerializeField]
	[Reorderable]
	public S13GlobalActionList actions;

	public void Execute()
	{
		S13Debug.Log("Executing Global Action group " + base.name, this, S13Debug.LogType.Action);
		foreach (S13GlobalAction action in this.actions)
		{
			action.Execute();
		}
	}

	public override string ToString()
	{
		return base.name;
	}
}
