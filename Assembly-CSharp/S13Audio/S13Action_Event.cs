using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_Event : IS13Executable
{
	[SerializeField]
	private S13ScriptableEvent eventToRaise;

	public bool IsExecutable => this.eventToRaise != null;

	public void Execute()
	{
		this.eventToRaise.Raise();
	}

	public override string ToString()
	{
		return "Event Raise " + (this.eventToRaise ? this.eventToRaise.name : "Empty Reference");
	}
}
