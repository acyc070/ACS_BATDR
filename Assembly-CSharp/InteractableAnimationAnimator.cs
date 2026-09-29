using System;
using UnityEngine;

[Serializable]
public class InteractableAnimationAnimator
{
	[SerializeField]
	public Animator Animator;

	[SerializeField]
	public AnimationClip AnimationClip;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public void Initialize()
	{
		if (!(this.Animator == null) && !(this.AnimationClip == null))
		{
			this.m_AnimatorOverrideController = new AnimatorOverrideController(this.Animator.runtimeAnimatorController);
			this.Animator.runtimeAnimatorController = this.m_AnimatorOverrideController;
			this.m_AimationClipOverrides = new AnimationClipOverrides(this.m_AnimatorOverrideController.overridesCount);
			this.m_AnimatorOverrideController.GetOverrides(this.m_AimationClipOverrides);
			if (!(this.m_AnimatorOverrideController == null) && this.m_AimationClipOverrides != null)
			{
				this.m_AnimatorOverrideController.ApplyOverrides(this.m_AimationClipOverrides);
			}
		}
	}

	public void Interact()
	{
		this.Animator.SetTrigger("Interact");
	}

	public void InteractEnter()
	{
		this.Animator.SetTrigger("InteractEnter");
	}

	public void InteractEixt()
	{
		this.Animator.SetTrigger("InteractExit");
	}
}
