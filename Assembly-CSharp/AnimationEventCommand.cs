using System;
using S13Audio;

[Serializable]
public class AnimationEventCommand
{
	public string CommandName;

	public int Frame;

	public S13ObjectPrimitiveSelector Clip;

	public bool isTriggered;
}
