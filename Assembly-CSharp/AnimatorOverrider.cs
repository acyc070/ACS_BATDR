using UnityEngine;

public class AnimatorOverrider : JMonoBehaviour
{
	[Header("Animation Animator")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Animation Events")]
	[Rename("Events")]
	[SerializeField]
	private GenericAnimationEvents m_GenericAnimationEvents;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	public Animator Animator => this.m_Animator;

	public GenericAnimationEvents GenericAnimationEvents => this.m_GenericAnimationEvents;

	public override void Awake()
	{
		this.SetAnimator();
		this.SetGenericAnimatorEvents();
		this.UpdateAnimationOverrideController();
		this.UpdateClipOverrides(this.m_AnimationClipGroup);
	}

	private void SetAnimator()
	{
		if (this.m_Animator == null)
		{
			this.m_Animator = base.gameObject.GetComponent<Animator>();
		}
		if (this.m_Animator != null)
		{
			this.m_Animator.applyRootMotion = false;
			this.m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
	}

	private void SetGenericAnimatorEvents()
	{
		if (this.m_GenericAnimationEvents == null)
		{
			this.m_GenericAnimationEvents = base.gameObject.GetComponent<GenericAnimationEvents>();
		}
	}

	public void UpdateAnimatorController(RuntimeAnimatorController animatorController)
	{
		this.m_Animator.runtimeAnimatorController = animatorController;
	}

	public void UpdateAnimationOverrideController()
	{
		this.m_AnimatorOverrideController = new AnimatorOverrideController(this.Animator.runtimeAnimatorController);
		this.Animator.runtimeAnimatorController = this.m_AnimatorOverrideController;
		this.m_AimationClipOverrides = new AnimationClipOverrides(this.m_AnimatorOverrideController.overridesCount);
		this.m_AnimatorOverrideController.GetOverrides(this.m_AimationClipOverrides);
	}

	public void UpdateClipOverrides(params AnimationClipGroup[] clipGroups)
	{
		if (!(this.m_AnimatorOverrideController == null) && this.m_AimationClipOverrides != null && clipGroups != null && clipGroups.Length != 0)
		{
			foreach (AnimationClipGroup animationClipGroup in clipGroups)
			{
				this.m_AimationClipOverrides[animationClipGroup.Name] = animationClipGroup.AnimationClip;
			}
			this.ApplyOverrides();
		}
	}

	public void UpdateClipOverride(string _name, AnimationClip clip)
	{
		if (!(this.m_AnimatorOverrideController == null) && this.m_AimationClipOverrides != null && !(clip == null) && !(_name == ""))
		{
			this.m_AimationClipOverrides[_name] = clip;
		}
	}

	public void ApplyOverrides()
	{
		this.m_AnimatorOverrideController.ApplyOverrides(this.m_AimationClipOverrides);
	}

	protected override void OnDisposed()
	{
		this.m_AnimatorOverrideController = null;
		this.m_AimationClipOverrides = null;
		base.OnDisposed();
	}
}
