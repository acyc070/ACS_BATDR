using System;
using UnityEngine;

[Serializable]
public class AnimationClipGroup
{
	public string Name;

	public AnimationClip AnimationClip;

	public void Initialize()
	{
		this.AnimationClip.name = this.Name;
	}
}
