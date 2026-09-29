using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_SetScriptableState : IS13Executable
{
	public enum S13SetScriptableStateMode
	{
		SetValue = 0,
		SetToPrevious = 1
	}

	[SerializeField]
	private S13SetScriptableStateMode mode;

	[SerializeField]
	private S13ScriptableState state;

	public bool IsExecutable => true;

	public void Execute()
	{
		switch (this.mode)
		{
		case S13SetScriptableStateMode.SetValue:
			S13Manager.SetScriptableState(this.state);
			break;
		case S13SetScriptableStateMode.SetToPrevious:
			S13Manager.SetScriptableStateToPrevious();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public override string ToString()
	{
		string text;
		text = " Set ScriptableState ";
		return this.mode switch
		{
			S13SetScriptableStateMode.SetValue => text + " to " + ((this.state != null) ? this.state.name : "None"), 
			S13SetScriptableStateMode.SetToPrevious => text + " to Previous", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
