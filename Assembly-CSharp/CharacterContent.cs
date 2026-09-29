using System.Collections.Generic;
using S13Audio;
using UnityEngine;

public class CharacterContent : JMonoBehaviour
{
	[Header("Character Vision")]
	[SerializeField]
	private Transform m_EyeSight;

	[Header("Character Model")]
	[SerializeField]
	private GameObject m_Model;

	[Header("Animator")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Animation Events")]
	[Rename("Events")]
	[SerializeField]
	private GenericAnimationEvents m_GenericAnimationEvents;

	[Header("Animation Audio")]
	[SerializeField]
	private CharacterAudioSelector m_CharacterAudio;

	[Header("Animations")]
	[SerializeField]
	private AnimationClipGroup[] m_AnimationClipGroup;

	[Header("Initialize Settings")]
	[SerializeField]
	private bool m_OnAwake = true;

	private AnimatorOverrideController m_AnimatorOverrideController;

	private AnimationClipOverrides m_AimationClipOverrides;

	private RagdollController m_RagdollController;

	private bool m_IsInitialized;

	public Transform EyeSight => this.m_EyeSight;

	public GameObject Model => this.m_Model;

	public Animator Animator => this.m_Animator;

	public GenericAnimationEvents GenericAnimationEvents => this.m_GenericAnimationEvents;

	public Renderer[] ModelRenderers { get; private set; }

	public RagdollController RagdollController => this.m_RagdollController;

	public override void Awake()
	{
		if (this.m_OnAwake)
		{
			this.Initialize();
		}
	}

	public void Initialize()
	{
		if (this.m_IsInitialized)
		{
			return;
		}
		if (this.m_CharacterAudio != null)
		{
			S13AnimationPassthrough component;
			component = base.gameObject.GetComponent<S13AnimationPassthrough>();
			if (component != null)
			{
				GameObject gameObject;
				gameObject = this.m_CharacterAudio.Get();
				if (gameObject != null)
				{
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
					component.RegisterAnimationActions(gameObject.GetComponentsInChildren<S13AnimationActions>());
				}
			}
		}
		this.SetAnimationOverrideController();
		this.SoftInitialize();
	}

	public void SoftInitialize()
	{
		if (this.m_Model == null)
		{
			Transform transform;
			transform = base.transform.FindChildName("TPose");
			if (transform == null)
			{
				transform = base.transform.FindChildName("Tpose");
			}
			if (transform != null)
			{
				this.m_Model = transform.gameObject;
			}
		}
		if (this.m_Model != null)
		{
			this.ModelRenderers = this.m_Model.transform.GetComponentsInChildren<Renderer>();
		}
		for (int i = 0; i < this.ModelRenderers.Length; i = unchecked(i + 1))
		{
			Renderer obj;
			obj = this.ModelRenderers[i];
			if (obj is SkinnedMeshRenderer skinnedMeshRenderer)
			{
				skinnedMeshRenderer.updateWhenOffscreen = true;
			}
			obj.material.SetFloat("_PostHitGlow", 0.3f);
		}
		if (this.m_Animator == null)
		{
			this.m_Animator = base.gameObject.GetComponent<Animator>();
		}
		if (this.m_GenericAnimationEvents == null)
		{
			this.m_GenericAnimationEvents = base.gameObject.GetComponent<GenericAnimationEvents>();
		}
		if (this.m_RagdollController == null)
		{
			this.m_RagdollController = base.GetComponentInChildren<RagdollController>();
		}
		this.m_IsInitialized = true;
	}

	public void SetAnimationTrigger(string trigger)
	{
		this.m_Animator?.SetTrigger(trigger);
	}

	public void Attack()
	{
		this.m_Animator?.Attack();
	}

	public void AttackLunge()
	{
		this.m_Animator?.AttackLunge();
	}

	public void AttackRanged()
	{
		this.m_Animator?.AttackRanged();
	}

	public void AttackBehind()
	{
		this.m_Animator?.SetTrigger("AttackBehind");
	}

	public void Jump()
	{
		this.m_Animator?.Jump();
	}

	public void Alert()
	{
		this.m_Animator?.SetTrigger("Alert");
	}

	private void SetAnimationOverrideController()
	{
		this.m_AnimatorOverrideController = new AnimatorOverrideController(this.Animator.runtimeAnimatorController);
		this.Animator.runtimeAnimatorController = this.m_AnimatorOverrideController;
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

	public void UpdateClipOverride(AnimationClip clip)
	{
		string text;
		text = clip.name;
		this.m_AimationClipOverrides[text] = clip;
		this.m_AnimatorOverrideController.ApplyOverrides(this.m_AimationClipOverrides);
	}

	public void SetAnimatorController(RuntimeAnimatorController animatorController)
	{
		this.m_Animator.runtimeAnimatorController = animatorController;
	}

	public void SetActiveRenderers(bool active)
	{
		for (int i = 0; i < this.ModelRenderers.Length; i = unchecked(i + 1))
		{
			this.ModelRenderers[i].enabled = active;
		}
	}

	public virtual void UpdateMaterials()
	{
	}

	protected override void OnDisposed()
	{
		this.m_AnimatorOverrideController = null;
		this.m_AimationClipOverrides = null;
		this.ModelRenderers = null;
		this.m_RagdollController = null;
		base.OnDisposed();
	}
}
