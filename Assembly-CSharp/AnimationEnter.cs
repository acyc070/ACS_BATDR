using System;
using DG.Tweening;
using UnityEngine;

public class AnimationEnter : AnimationBase
{
	private const string INTERACT_ENTER = "InteractEnter";

	private const string INTERACT_ENTER_INSTANT = "InteractEnterInstant";

	private const string INTERACT_LOOP = "InteractLoop";

	private const string INTERACT_EXIT = "InteractExit";

	private const string INTERACT_EXIT_FORCE_COMPLETE = "InteractExitForceComplete";

	[SerializeField]
	private bool m_SkipEnter;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	[Header("Input Exit")]
	[SerializeField]
	private bool m_InputExit = true;

	[SerializeField]
	private InteractionType m_InteractionType;

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

	private Sequence m_HeadRotationSequence;

	private bool m_IsInteracted;

	private bool m_IsLocked;

	private void Update()
	{
		if (!this.m_IsLocked && this.m_InputExit && this.m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && this.m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			this.Exit();
		}
	}

	public override void Enter()
	{
		if (base.m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		if (this.m_InputExit)
		{
			string text;
			text = TextUtility.GetKey(this.m_InteractionType.ToString());
			if (base.m_IsReal)
			{
				text = text.ToUpper();
			}
			GameManager.Instance.ShowInteraction(text, base.m_IsReal);
		}
		GameManager.Instance.Player.HideFirstPersonArms();
		if (!this.m_SkipEnter)
		{
			this.m_InteractEnterClip.name = "InteractEnter";
		}
		this.m_InteractLoopClip.name = "InteractLoop";
		this.m_InteractExitClip.name = "InteractExit";
		this.RemoveListeners();
		unchecked
		{
			if (!this.m_SkipEnter)
			{
				GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_InteractEnterClip, this.m_InteractLoopClip, this.m_InteractExitClip);
				GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
				GameManager.Instance.Player.EnterInteraction("InteractEnter");
				for (int i = 0; i < this.m_Animators.Length; i++)
				{
					this.m_Animators[i].SetTrigger("InteractEnter");
				}
			}
			else
			{
				GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_InteractLoopClip, this.m_InteractExitClip);
				GameManager.Instance.Player.EnterInteractionInstant("InteractEnterInstant");
				for (int j = 0; j < this.m_Animators.Length; j++)
				{
					this.m_Animators[j].SetTrigger("InteractEnterInstant");
				}
			}
			Vector3 vector;
			vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			base.ResetSequence();
			base.m_Sequence.SetUpdate(UpdateType.Fixed);
			if (!this.m_SkipEnter)
			{
				base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(base.m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
				base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(base.m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
				return;
			}
			GameManager.Instance.Player.transform.position = base.m_StartLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = base.m_StartLocation.eulerAngles;
			base.m_Sequence.InsertCallback(0.1f, () =>
			{
				this.m_IsInteracted = true;
				if (this.m_EnableAnimationRotation)
				{
					GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y);
				}
				base.SendOnComplete();
			});
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			this.RemoveListeners();
			this.m_IsInteracted = true;
			if (this.m_EnableAnimationRotation)
			{
				GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y);
			}
			base.SendOnComplete();
		}
	}

	public override void Exit()
	{
		this.m_IsInteracted = false;
		if (this.m_InputExit)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.ClearInteraction();
		}
		for (int i = 0; i < this.m_Animators.Length; i = unchecked(i + 1))
		{
			this.m_Animators[i].SetTrigger("InteractExit");
		}
		this.RemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += this.HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		if (this.m_EnableAnimationRotation)
		{
			base.ResetSequence();
			base.m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InOutSine));
		}
	}

	public void ForceExit()
	{
		for (int i = 0; i < this.m_Animators.Length; i = unchecked(i + 1))
		{
			this.m_Animators[i].SetTrigger("InteractExitForceComplete");
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		if (!base.IsDisposed)
		{
			this.RemoveListeners();
			if (base.HasEndLocation)
			{
				Vector3 vector;
				vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				GameManager.Instance.Player.transform.position = base.m_EndLocation.position + vector;
				GameManager.Instance.Player.transform.eulerAngles = base.m_EndLocation.eulerAngles;
			}
			if (base.m_HideCrosshair)
			{
				GameManager.Instance.ShowCrosshair(base.m_IsReal);
			}
			GameManager.Instance.Player.ExitAnimation();
			base.SendOnExit();
		}
	}

	public void ResetCameraRotation()
	{
		this.KillHeadRotationSequence();
		this.m_HeadRotationSequence = DOTween.Sequence();
		this.m_HeadRotationSequence.Insert(0f, GameManager.Instance.Player.HeadContainer.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine));
		this.m_HeadRotationSequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine));
	}

	public void SetLock(bool isLocked)
	{
		this.m_IsLocked = isLocked;
	}

	public void ResetInteraction()
	{
		this.RemoveListeners();
		this.m_IsInteracted = false;
	}

	private void KillHeadRotationSequence()
	{
		if (this.m_HeadRotationSequence != null)
		{
			this.m_HeadRotationSequence.Kill();
			this.m_HeadRotationSequence = null;
		}
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
		this.KillHeadRotationSequence();
		base.OnDisposed();
	}
}
