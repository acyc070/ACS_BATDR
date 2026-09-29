using System;
using UnityEngine;

[Serializable]
public class CharacterAnimationEventData
{
	public string Name;

	public AnimationClip Clip;

	public AnimationEventCommands AnimationEventCommands = new AnimationEventCommands();
}
