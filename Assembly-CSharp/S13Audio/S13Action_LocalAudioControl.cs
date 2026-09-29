using System;

namespace S13Audio;

[Serializable]
public class S13Action_LocalAudioControl : S13Action_AudioControl<S13AudioReference>
{
	public override bool IsExecutable
	{
		get
		{
			if (base.IsExecutable)
			{
				return base.audio.Reference != null;
			}
			return false;
		}
	}
}
