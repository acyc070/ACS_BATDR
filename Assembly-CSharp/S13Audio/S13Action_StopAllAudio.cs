using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_StopAllAudio : IS13Executable
{
	[Tooltip("Should the audio cut off?")]
	[SerializeField]
	public bool audioIgnoreFades;

	public bool IsExecutable => true;

	public void Execute()
	{
		S13Manager.StopAll(this.audioIgnoreFades);
	}

	public override string ToString()
	{
		return $"StopAllAudio| Ignore Fades:{this.audioIgnoreFades}";
	}
}
