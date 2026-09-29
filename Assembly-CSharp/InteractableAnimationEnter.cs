using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationEnter : InteractableAnimationBase
{
	private const string INTERACT_ENTER = "InteractEnter";

	private const string INTERACT_ENTER_INSTANT = "InteractEnterInstant";

	private const string INTERACT_LOOP = "InteractLoop";

	private const string INTERACT_EXIT = "InteractExit";

	[SerializeField]
	private bool m_EndLocationOnIdle;

	[SerializeField]
	private bool m_InputExit = true;

	[SerializeField]
	private bool m_EndCrouched;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private bool m_IsCutscene;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	[Header("Animation Clip")]
	[SerializeField]
	private AnimationClip m_InteractEnterClip;

	[SerializeField]
	private AnimationClip m_InteractLoopClip;

	[SerializeField]
	private AnimationClip m_InteractExitClip;

	[Header("Animation Props")]
	[SerializeField]
	private Animator[] m_Animators;

	private bool m_IsInteracted;

	private bool m_IsLocked;

	public bool IsInteracted => this.m_IsInteracted;

	private void Update()
	{
		if (!this.m_IsLocked && this.m_InputExit && this.m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && this.m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			this.Exit();
		}
	}

	public override void Enter()
	{
		base.CheckCrosshair(show: false);
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		base.ResetSequence();
		base.m_Sequence.SetUpdate(UpdateType.Fixed);
		base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(base.m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
		base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(base.m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
		this.RemoveListeners();
		this.m_InteractEnterClip.name = "InteractEnter";
		this.m_InteractLoopClip.name = "InteractLoop";
		this.m_InteractExitClip.name = "InteractExit";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_InteractEnterClip, this.m_InteractLoopClip, this.m_InteractExitClip);
		GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction("InteractEnter");
		for (int i = 0; i < this.m_Animators.Length; i = unchecked(i + 1))
		{
			this.m_Animators[i].SetTrigger("InteractEnter");
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			this.RemoveListeners();
			this.m_IsInteracted = true;
			if (this.m_EndLocationOnIdle && base.HasEndLocation)
			{
				base.SendToEndLocation();
			}
			if (this.m_EnableAnimationRotation)
			{
				GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y, this.m_IsCutscene);
			}
			base.SendOnInteractionComplete();
		}
	}

	public override void ForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
		base.StartCoroutine(this.InternalForceEnter(rotationX, rotationY));
	}

	private IEnumerator InternalForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		base.SendOnInteract(this);
		base.CheckCrosshair(show: false);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.Player.HideFirstPersonArms();
		this.m_InteractLoopClip.name = "InteractLoop";
		this.m_InteractExitClip.name = "InteractExit";
		this.RemoveListeners();
		this.m_IsInteracted = true;
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_InteractLoopClip, this.m_InteractExitClip);
		GameManager.Instance.Player.EnterInteractionInstant("InteractEnterInstant");
		for (int i = 0; i < this.m_Animators.Length; i = unchecked(i + 1))
		{
			this.m_Animators[i].SetTrigger("InteractEnterInstant");
		}
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		GameManager.Instance.Player.transform.position = base.m_StartLocation.position + vector;
		GameManager.Instance.Player.transform.eulerAngles = base.m_StartLocation.eulerAngles;
		if (this.m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y, this.m_IsCutscene);
		}
		GameManager.Instance.Player.ForceRotation(rotationX, rotationY);
		base.SendOnInteractionComplete();
	}

	public override void Exit()
	{
		if (this.m_EndCrouched)
		{
			GameManager.Instance.Player.ForceCrouch(isInternalCrouch: false);
		}
		else
		{
			GameManager.Instance.Player.ForceStand(isOutOfCombat: false);
		}
		this.m_IsInteracted = false;
		unchecked
		{
			for (int i = 0; i < this.m_Animators.Length; i++)
			{
				this.m_Animators[i].SetTrigger("InteractExit");
			}
			this.RemoveListeners();
			GameManager.Instance.Player.OnAnimationComplete += this.HandleExitOnAnimationComplete;
			GameManager.Instance.Player.ExitInteraction();
			if (this.m_EnableAnimationRotation)
			{
				base.ResetSequence();
				base.m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
			}
			if (base.m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
			{
				GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[(int)base.m_presetRumble]);
			}
			else
			{
				GameManager.Instance.TriggerRumble(base.m_CustomRumble);
			}
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			this.RemoveListeners();
			base.SendToEndLocation();
			if (!base.m_IsSingleInteraction)
			{
				base.ResetAction();
				this.SetActive(active: true);
			}
			GameManager.Instance.Player.ExitAnimation();
			base.CheckCrosshair(show: true);
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.EnableTeleport();
			}
			base.SendOnInteractionExit();
		}
	}

	public void SetLock(bool isLocked)
	{
		this.m_IsLocked = isLocked;
	}

	public void ResetInteraction()
	{
		this.RemoveListeners();
		this.m_IsInteracted = true;
	}

	private void RemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
