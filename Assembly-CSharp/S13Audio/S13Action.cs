using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace S13Audio;

[Serializable]
public abstract class S13Action<TAudio, TS13ActionGroup> : S13Action where TAudio : S13Action_AudioControl where TS13ActionGroup : S13Action_Group
{
	[Tooltip("The main function type of this Action.")]
	[SerializeField]
	public ActionType type;

	[SerializeField]
	public S13Action_StopAllAudio StopAllAudio;

	[SerializeField]
	[FormerlySerializedAs("ManageSoundBank")]
	public S13Action_SoundBank Soundbank;

	[FormerlySerializedAs("MixerSnapshot")]
	[SerializeField]
	public S13Action_SetMixer Snapshot;

	[SerializeField]
	public UnityEvent UnityEvent;

	[FormerlySerializedAs("SetSwitch")]
	[SerializeField]
	public S13Action_SetSwitch Switch = new S13Action_SetSwitch
	{
		switchType = (S13Action<TAudio, TS13ActionGroup>.IsLocal ? S13Action_SetSwitch.SwitchType.Local : S13Action_SetSwitch.SwitchType.Global)
	};

	[FormerlySerializedAs("SetParameter")]
	[FormerlySerializedAs("Parameter")]
	[SerializeField]
	public S13Action_SetAccessor Accessor;

	[SerializeField]
	[FormerlySerializedAs("SequencerControl")]
	public S13Action_SequencerControl Sequencer;

	[SerializeField]
	public S13Action_Event Event;

	[SerializeField]
	public S13Action_SetGameState SetGamePauseState;

	[SerializeField]
	[FormerlySerializedAs("ControlAudio")]
	public TAudio Audio;

	[SerializeField]
	[FormerlySerializedAs("ActionGroup")]
	public TS13ActionGroup ActionList;

	public S13Action_SetScriptableState SetScriptableState;

	private IS13Executable TargetAction => this.type switch
	{
		ActionType.None => (IS13Executable)null, 
		ActionType.Audio => this.Audio, 
		ActionType.StopAllAudio => this.StopAllAudio, 
		ActionType.Soundbank => this.Soundbank, 
		ActionType.Snapshot => this.Snapshot, 
		ActionType.ActionList => this.ActionList, 
		ActionType.UnityEvent => null, 
		ActionType.Switch => this.Switch, 
		ActionType.Accessor => this.Accessor, 
		ActionType.Sequencer => this.Sequencer, 
		ActionType.Event => this.Event, 
		ActionType.SetGamePauseState => this.SetGamePauseState, 
		ActionType.SetScriptableState => this.SetScriptableState, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	private static bool IsLocal
	{
		get
		{
			if (typeof(TAudio) == typeof(S13Action_LocalAudioControl))
			{
				return typeof(TS13ActionGroup) == typeof(S13Action_LocalActionGroup);
			}
			return false;
		}
	}

	public override bool IsExecutable
	{
		get
		{
			if (this.type == ActionType.UnityEvent)
			{
				return true;
			}
			return this.TargetAction?.IsExecutable ?? false;
		}
	}

	public override void Execute()
	{
		if (this.type == ActionType.None)
		{
			return;
		}
		S13Debug.Log(this.GenerateLog() ?? "", S13Debug.LogType.Action);
		if (this.TargetAction != null)
		{
			if (this.TargetAction.IsExecutable)
			{
				this.TargetAction.Execute();
			}
		}
		else if (this.type == ActionType.UnityEvent)
		{
			this.UnityEvent.Invoke();
		}
	}

	private string GenerateLog()
	{
		return "";
	}
}
[Serializable]
public abstract class S13Action : IS13Executable
{
	public enum ActionType
	{
		None = 0,
		Audio = 1,
		StopAllAudio = 2,
		Soundbank = 3,
		Snapshot = 4,
		SetGamePauseState = 5,
		ActionList = 6,
		UnityEvent = 7,
		Switch = 8,
		Accessor = 9,
		Sequencer = 10,
		Event = 11,
		SetScriptableState = 12
	}

	public virtual bool IsExecutable => false;

	public abstract void Execute();
}
