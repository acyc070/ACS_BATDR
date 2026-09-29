using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_SoundBank : IS13Executable
{
	[Serializable]
	public enum SoundBankControl
	{
		None = 0,
		Load = 1,
		Unload = 2,
		Stop = 3
	}

	[Tooltip("The action to take on the sound bank.")]
	[SerializeField]
	private SoundBankControl soundBankControl;

	[Tooltip("The sound bank to operate on.")]
	[SerializeField]
	private S13SoundBank soundBank;

	[Tooltip("The delay before unloading the SoundBank")]
	[SerializeField]
	private float delayTime;

	public bool IsExecutable
	{
		get
		{
			if (this.soundBank != null)
			{
				return this.soundBankControl != SoundBankControl.None;
			}
			return false;
		}
	}

	public void Execute()
	{
		switch (this.soundBankControl)
		{
		case SoundBankControl.Load:
			S13Manager.LoadSoundBank(this.soundBank);
			break;
		case SoundBankControl.Unload:
			if (this.delayTime > 0f)
			{
				S13Manager.UnloadSoundBankDelayed(this.soundBank, this.delayTime);
			}
			else
			{
				S13Manager.UnloadSoundBank(this.soundBank);
			}
			break;
		case SoundBankControl.Stop:
			S13Manager.StopSoundBank(this.soundBank);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public override string ToString()
	{
		if (this.soundBankControl != SoundBankControl.Unload || !(this.delayTime > 0f))
		{
			return $" SoundBank: {this.soundBankControl} {this.soundBank?.name}";
		}
		return $" SoundBank: {this.soundBankControl} {this.soundBank?.name} {this.delayTime}";
	}
}
