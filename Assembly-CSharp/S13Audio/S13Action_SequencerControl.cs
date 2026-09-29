using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_SequencerControl : IS13Executable
{
	private enum ControlType
	{
		None = 0,
		Play = 1,
		Stop = 2,
		TransitionImmediate = 3,
		TransitionNextBar = 4,
		TransitionNextBeat = 5,
		PlayStingerNextBeat = 6,
		PlayStingerNextBar = 7
	}

	[SerializeField]
	private S13MusicSequencer sequencer;

	[SerializeField]
	private ControlType controlType;

	public bool IsExecutable
	{
		get
		{
			if (this.sequencer != null)
			{
				return this.controlType != ControlType.None;
			}
			return false;
		}
	}

	public override string ToString()
	{
		return $"SequencerControl {this.sequencer?.name} {this.controlType}";
	}

	public void Execute()
	{
		switch (this.controlType)
		{
		case ControlType.Play:
			this.sequencer.Play();
			break;
		case ControlType.Stop:
			this.sequencer.Stop();
			break;
		case ControlType.TransitionImmediate:
			this.sequencer.TransitionImmediate();
			break;
		case ControlType.TransitionNextBar:
			this.sequencer.TransitionNextBar();
			break;
		case ControlType.TransitionNextBeat:
			this.sequencer.TransitionNextBeat();
			break;
		case ControlType.PlayStingerNextBeat:
			this.sequencer.PlayStingerNextBeat();
			break;
		case ControlType.PlayStingerNextBar:
			this.sequencer.PlayStingerNextBar();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ControlType.None:
			break;
		}
	}
}
