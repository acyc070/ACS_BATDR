using System;
using DG.Tweening;
using UnityEngine;

public class EventTriggerAnimation : JMonoBehaviour
{
	[Serializable]
	public class EventTriggerAnimator
	{
		[SerializeField]
		public string Trigger;

		[SerializeField]
		public Animator Animator;
	}

	[SerializeField]
	private string m_AnimationTrigger;

	[SerializeField]
	private LayerMask m_IgnoreLayer;

	[Header("Props")]
	[SerializeField]
	private EventTriggerAnimator[] m_EventTriggerAnimators;

	[Header("Locations")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private bool m_CanOffsetLocations = true;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private Collider m_TargetCollider;

	[Header("Other Options")]
	[SerializeField]
	private bool m_HideCrosshair;

	[SerializeField]
	private bool m_IsReal;

	[SerializeField]
	private bool m_SnapOnComplete;

	private Vector3 m_InitialStartLocation;

	private Vector3 m_InitialEndLocation;

	private Sequence m_Sequence;

	private Transform m_Camera => GameManager.Instance.GameCamera.Camera.transform;

	public bool HasStartLocation => this.m_StartLocation != null;

	public bool HasEndLocation => this.m_EndLocation != null;

	public event EventHandler OnTriggered;

	public override void Start()
	{
		this.SetInitialLocations();
		this.AddListeners();
	}

	private void HandleTriggerEventOnEnter(object sender, EventArgs e)
	{
		if (!GameManager.Instance.Player.IsGrounded)
		{
			GameManager.Instance.Player.ResetAnimationTrigger("Jump");
		}
		this.RemoveListeners();
		if (this.m_TargetCollider != null)
		{
			Physics.Raycast(this.m_Camera.position, this.m_Camera.forward, out var hitInfo, 10f, ~(int)this.m_IgnoreLayer);
			if (hitInfo.transform != this.m_TargetCollider.transform)
			{
				this.ResetInitialLocations();
				this.m_EventTrigger.ResetAction();
				this.AddListeners();
				return;
			}
		}
		if (this.m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		if (this.m_CanOffsetLocations)
		{
			this.CheckStartLocation(this.m_Camera);
		}
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandleOnAnimationComplete;
		GameManager.Instance.Player.EnterInteraction(this.m_AnimationTrigger);
		for (int i = 0; i < this.m_EventTriggerAnimators.Length; i = unchecked(i + 1))
		{
			EventTriggerAnimator eventTriggerAnimator;
			eventTriggerAnimator = this.m_EventTriggerAnimators[i];
			eventTriggerAnimator.Animator.SetTrigger(eventTriggerAnimator.Trigger);
		}
		if (this.HasStartLocation)
		{
			this.ResetSequence();
			this.m_Sequence.SetUpdate(UpdateType.Fixed);
			this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.m_StartLocation.eulerAngles, 0.1f).SetEase(Ease.InOutSine));
			this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.m_StartLocation.position, 0.1f).SetEase(Ease.InOutSine));
			this.m_Sequence.OnComplete(() =>
			{
				GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
			});
		}
		else
		{
			GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		}
		this.OnTriggered.Send(this);
	}

	private void HandleOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleOnAnimationComplete;
		if (this.HasEndLocation)
		{
			Vector3 vector;
			vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = this.m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = this.m_EndLocation.eulerAngles;
		}
		this.ResetInitialLocations();
		this.m_EventTrigger.ResetAction();
		this.AddListeners();
		if (this.m_HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair(this.m_IsReal);
		}
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.ExitAnimation();
	}

	private void SetInitialLocations()
	{
		if (this.HasStartLocation)
		{
			this.m_InitialStartLocation = this.m_StartLocation.localPosition;
		}
		if (this.HasEndLocation)
		{
			this.m_InitialEndLocation = this.m_EndLocation.localPosition;
		}
	}

	private void ResetInitialLocations()
	{
		if (this.HasStartLocation)
		{
			this.m_StartLocation.localPosition = this.m_InitialStartLocation;
		}
		if (this.HasEndLocation)
		{
			this.m_EndLocation.localPosition = this.m_InitialEndLocation;
		}
	}

	private void CheckStartLocation(Transform cameraTransform)
	{
		float x;
		x = 0f - cameraTransform.InverseTransformPoint(this.m_StartLocation.position).x;
		if (this.HasStartLocation)
		{
			Vector3 localPosition;
			localPosition = this.m_StartLocation.localPosition;
			localPosition.x = x;
			this.m_StartLocation.localPosition = localPosition;
		}
		if (this.HasEndLocation)
		{
			Vector3 localPosition2;
			localPosition2 = this.m_EndLocation.localPosition;
			localPosition2.x = x;
			this.m_EndLocation.localPosition = localPosition2;
		}
	}

	private void AddListeners()
	{
		this.m_EventTrigger.OnEnter += this.HandleTriggerEventOnEnter;
	}

	private void RemoveListeners()
	{
		this.m_EventTrigger.OnEnter -= this.HandleTriggerEventOnEnter;
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.KillSequence();
		base.OnDisposed();
	}
}
