using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverriderGroup : JMonoBehaviour
{
	[SerializeField]
	private AnimatorOverrider m_AnimatorOverrider;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	public override void Start()
	{
		for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroup[i].Initialize();
		}
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClipGroup> list;
		list = new List<AnimationClipGroup>();
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i++)
			{
				AnimationClipOverrideGroup animationClipOverrideGroup;
				animationClipOverrideGroup = this.m_AnimationClipOverrideGroup[i];
				if (animationClipOverrideGroup.Name == name)
				{
					for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
					{
						list.Add(animationClipOverrideGroup.AnimationClipGroups[j]);
					}
					break;
				}
			}
			this.m_AnimatorOverrider.UpdateClipOverrides(list.ToArray());
		}
	}
}
