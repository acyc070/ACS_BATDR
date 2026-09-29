using System;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationSingle : InteractableAnimationBase
{
	private const string INTERACT = "Interact";

	private const string INTERACT_INSTANT = "InteractInstant";

	[Header("Animation Clip")]
	[SerializeField]
	private AnimationClip m_InteractClip;

	[Header("Animation Props")]
	[SerializeField]
	private bool m_InstantInteract;

	[SerializeField]
	private Animator[] m_Animators;

	public Animator[] Animators => this.m_Animators;

	public void UpdateInteractClip(AnimationClip animationClip)
	{
		this.m_InteractClip = animationClip;
	}

	public override void Enter()
	{
		base.CheckCrosshair(show: false);
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		base.ResetSequence();
		base.m_Sequence.SetUpdate(UpdateType.Fixed);
		base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(base.m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
		base.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(base.m_StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine));
		this.RemoveListeners();
		this.m_InteractClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_InteractClip);
		GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction("Interact");
		for (int i = 0; i < this.m_Animators.Length; i = unchecked(i + 1))
		{
			this.m_Animators[i].SetTrigger(this.m_InstantInteract ? "InteractInstant" : "Interact");
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendToEndLocation();
		if (!base.m_IsSingleInteraction)
		{
			base.ResetAction();
			this.SetActive(active: true);
		}
		base.CheckCrosshair(show: true);
		GameManager.Instance.Player.ExitAnimation();
		base.SendOnInteractionComplete();
	}

	private void RemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
