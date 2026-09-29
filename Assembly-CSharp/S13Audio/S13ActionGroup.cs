using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13ActionGroup<TActionGroup> : S13Action_Group where TActionGroup : IS13ActionGroup
{
	[Tooltip("The action group to execute.")]
	[SerializeField]
	protected TActionGroup commandGroup;

	public override bool IsExecutable => this.commandGroup != null;

	public override void Execute()
	{
		this.commandGroup.Execute();
	}

	public override string ToString()
	{
		if (this.commandGroup == null)
		{
			return " Empty actionGroup";
		}
		return " actionGroup: " + this.commandGroup.ToString();
	}
}
