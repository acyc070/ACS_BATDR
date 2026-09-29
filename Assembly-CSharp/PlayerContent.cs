using System.Collections.Generic;
using UnityEngine;

public class PlayerContent : JMonoBehaviour
{
	[Header("Animation")]
	[SerializeField]
	private Animator m_Animator;

	[Space]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public Animator Animator => this.m_Animator;

	public override void Awake()
	{
		if (this.m_Animator == null)
		{
			this.m_Animator = base.gameObject.GetComponent<Animator>();
		}
		this.SetAnimationOverrideController();
	}

	private void SetAnimationOverrideController()
	{
		this.m_AnimatorOverrideController = new AnimatorOverrideController(this.m_Animator.runtimeAnimatorController);
		this.m_Animator.runtimeAnimatorController = this.m_AnimatorOverrideController;
		this.m_AimationClipOverrides = new AnimationClipOverrides(this.m_AnimatorOverrideController.overridesCount);
		this.m_AnimatorOverrideController.GetOverrides(this.m_AimationClipOverrides);
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipGroup.Length; i++)
			{
				this.m_AnimationClipGroup[i].Initialize();
			}
			List<AnimationClip> list;
			list = new List<AnimationClip>();
			for (int j = 0; j < this.m_AnimationClipGroup.Length; j++)
			{
				list.Add(this.m_AnimationClipGroup[j].AnimationClip);
			}
			this.UpdateClipOverrides(list.ToArray());
		}
	}

	public void UpdateClipOverrides(params AnimationClip[] clips)
	{
		if (!(this.m_AnimatorOverrideController == null) && this.m_AimationClipOverrides != null && clips != null && clips.Length != 0)
		{
			foreach (AnimationClip animationClip in clips)
			{
				string text;
				text = animationClip.name;
				this.m_AimationClipOverrides[text] = animationClip;
			}
			this.m_AnimatorOverrideController.ApplyOverrides(this.m_AimationClipOverrides);
		}
	}

	protected override void OnDisposed()
	{
		this.m_AnimatorOverrideController = null;
		this.m_AimationClipOverrides = null;
		base.OnDisposed();
	}
}
