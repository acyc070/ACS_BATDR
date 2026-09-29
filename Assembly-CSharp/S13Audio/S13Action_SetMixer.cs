using System;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

[Serializable]
public class S13Action_SetMixer : IS13Executable
{
	[Tooltip("The audio mixer with the target snapshot.")]
	[SerializeField]
	private AudioMixer mixer;

	[Tooltip("The snapshot to transition the mixer to.")]
	[SerializeField]
	private AudioMixerSnapshot mixerSnapshot;

	[Tooltip("The time the mixer transition should take place over.")]
	[SerializeField]
	private float mixerTransitionTime;

	public bool IsExecutable
	{
		get
		{
			if (this.mixer != null)
			{
				return this.mixerSnapshot != null;
			}
			return false;
		}
	}

	public void Execute()
	{
		if (this.mixerSnapshot != null)
		{
			this.mixerSnapshot.TransitionTo(this.mixerTransitionTime);
		}
	}

	public override string ToString()
	{
		if (!(this.mixerSnapshot == null))
		{
			return $" Snapshot:{this.mixerSnapshot.name} TransitionTime: {this.mixerTransitionTime}";
		}
		return $" Snapshot: Empty Reference TransitionTime: {this.mixerTransitionTime}";
	}
}
