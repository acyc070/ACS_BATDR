using System;

[Serializable]
public class AnimationClipOverrideGroup
{
	public string Name;

	public AnimationClipGroup[] AnimationClipGroups;

	public void Initialize()
	{
		for (int i = 0; i < this.AnimationClipGroups.Length; i = unchecked(i + 1))
		{
			this.AnimationClipGroups[i].Initialize();
		}
	}
}
