using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimation : JMonoBehaviour
{
	[Serializable]
	public class InteractableAnimator
	{
		[SerializeField]
		public string Trigger;

		[SerializeField]
		public Animator Animator;

		[SerializeField]
		public bool DisableOnComplete;
	}

	[Header("Player")]
	[SerializeField]
	private string m_AnimationTrigger;

	[SerializeField]
	private Interactable m_Interactable;

	[Header("Props")]
	[SerializeField]
	private InteractableAnimator[] m_InteractableEnterAnimators;

	[SerializeField]
	private InteractableAnimator[] m_InteractableExitAnimators;

	[Header("Positions")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private Transform m_CancelLocation;

	[SerializeField]
	private Transform m_SlideLocation;

	[SerializeField]
	private bool m_UseDynamicLocation;

	[SerializeField]
	private float m_DynamicLocationDistance = 2.42f;

	[Header("Options")]
	[SerializeField]
	private bool m_EnableOnStart = true;

	[SerializeField]
	private bool m_IsSingleInteraction;

	[SerializeField]
	private bool m_IsSingleInteractionExit;

	[SerializeField]
	private string m_AnimationExitTrigger;

	[SerializeField]
	private bool m_DisableInteractionButtonExit;

	[SerializeField]
	private bool IgnoreSlideToLocation;

	[SerializeField]
	private bool SlideAndPlayImmediately;

	[SerializeField]
	private bool m_HideCrosshair;

	[SerializeField]
	private bool m_IsReal;

	[SerializeField]
	private bool m_EnableAnimationRotation = true;

	[SerializeField]
	private Vector2 m_RotationLock = new Vector2(15f, 20f);

	private bool m_IsInteracted;

	private Vector3 m_PlayerPosition;

	private Sequence m_Sequence;

	public bool CanCancel => this.m_CancelLocation != null;

	public bool HasStartLocation => this.m_StartLocation != null;

	public bool HasEndLocation => this.m_EndLocation != null;

	public bool IsLocked { get; private set; }

	public bool IsSingleInteraction => this.m_IsSingleInteraction;

	public bool HideCrosshair => this.m_HideCrosshair;

	public Interactable Interactable => this.m_Interactable;

	public Transform StartLocation => this.m_StartLocation;

	public event EventHandler OnInteracted;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnInteractionOnEnter;

	public event EventHandler OnInteractionOnExit;

	public override void Start()
	{
		this.SetActive(this.m_EnableOnStart);
		this.m_Interactable.OnEnter += this.HandleInteractableOnEnter;
		this.m_Interactable.OnExit += this.HandleInteractableOnExit;
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		this.OnInteractionOnExit.Send(this);
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		this.OnInteractionOnEnter.Send(this);
	}

	private void Update()
	{
		if (!this.IsLocked && this.m_IsInteracted && (this.CanCancel || this.HasEndLocation) && !this.m_DisableInteractionButtonExit && !base.IsDisposed && !GameManager.Instance.IsPaused && this.m_IsInteracted && PlayerInput.InteractOnReleased())
		{
			if (this.CanCancel && PlayerInput.MoveY() > 0.1f)
			{
				this.Cancel();
			}
			else
			{
				this.Exit();
			}
		}
	}

	public void ResetAll()
	{
		this.m_IsInteracted = false;
		this.RemoveListeners();
		this.SetActive(this.m_EnableOnStart);
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			this.AddListeners();
		}
		else
		{
			this.RemoveListeners();
		}
		this.m_Interactable.SetActive(active);
	}

	public void SetLocked(bool isLocked)
	{
		this.IsLocked = isLocked;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.RemoveListeners();
		this.Enter();
	}

	public void Enter()
	{
		if (this.m_UseDynamicLocation)
		{
			Vector3 vector;
			vector = GameManager.Instance.Player.transform.position - base.transform.position;
			Vector3 position;
			position = base.transform.position + vector.normalized * this.m_DynamicLocationDistance;
			position.y = GameManager.Instance.Player.transform.position.y;
			this.m_StartLocation.position = position;
			Vector3 position2;
			position2 = base.transform.position;
			position2.y = position.y;
			this.m_StartLocation.LookAt(position2);
			if (base.GetComponent<Rigidbody>() != null)
			{
				base.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		if (this.m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		unchecked
		{
			if (this.HasStartLocation && this.IgnoreSlideToLocation)
			{
				if (this.m_SlideLocation == null)
				{
					GameManager.Instance.Player.HeadContainer.SetParent(null);
				}
				else
				{
					GameManager.Instance.Player.SetState(State.Player.Cutscene);
					Vector3 vector2;
					vector2 = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
					this.ResetSequence();
					this.m_Sequence.SetUpdate(UpdateType.Fixed);
					this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.m_SlideLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
					this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.m_SlideLocation.position + vector2, 0.25f).SetEase(Ease.InOutSine));
					if (this.SlideAndPlayImmediately)
					{
						GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
						GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
						GameManager.Instance.Player.EnterInteraction(this.m_AnimationTrigger);
						for (int i = 0; i < this.m_InteractableEnterAnimators.Length; i++)
						{
							InteractableAnimator interactableAnimator;
							interactableAnimator = this.m_InteractableEnterAnimators[i];
							interactableAnimator.Animator.gameObject.SetActive(value: true);
							interactableAnimator.Animator.SetTrigger(interactableAnimator.Trigger);
						}
					}
					else
					{
						this.m_Sequence.Insert(0f, GameManager.Instance.Player.HeadContainer.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InOutSine));
						this.m_Sequence.OnComplete(() =>
						{
							GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
							GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
							GameManager.Instance.Player.EnterInteractionInstant(this.m_AnimationTrigger);
							for (int j = 0; j < this.m_InteractableEnterAnimators.Length; j++)
							{
								InteractableAnimator interactableAnimator3;
								interactableAnimator3 = this.m_InteractableEnterAnimators[j];
								interactableAnimator3.Animator.gameObject.SetActive(value: true);
								interactableAnimator3.Animator.SetTrigger(interactableAnimator3.Trigger);
							}
							GameManager.Instance.Player.transform.position = this.m_StartLocation.position;
							GameManager.Instance.Player.transform.eulerAngles = this.m_StartLocation.eulerAngles;
						});
					}
				}
			}
			if (this.m_SlideLocation == null)
			{
				GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
				GameManager.Instance.Player.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
				GameManager.Instance.Player.EnterInteraction(this.m_AnimationTrigger);
				for (int num = 0; num < this.m_InteractableEnterAnimators.Length; num++)
				{
					InteractableAnimator interactableAnimator2;
					interactableAnimator2 = this.m_InteractableEnterAnimators[num];
					interactableAnimator2.Animator.gameObject.SetActive(value: true);
					interactableAnimator2.Animator.SetTrigger(interactableAnimator2.Trigger);
				}
			}
			if (this.HasStartLocation)
			{
				if (!this.IgnoreSlideToLocation)
				{
					Vector3 vector3;
					vector3 = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
					this.ResetSequence();
					this.m_Sequence.SetUpdate(UpdateType.Fixed);
					this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.m_StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine));
					this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.m_StartLocation.position + vector3, 0.25f).SetEase(Ease.InOutSine)).OnComplete(() =>
					{
						GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
					});
				}
				else
				{
					GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
					if (this.m_SlideLocation == null)
					{
						GameManager.Instance.Player.transform.position = this.m_StartLocation.position;
						GameManager.Instance.Player.transform.eulerAngles = this.m_StartLocation.eulerAngles;
					}
				}
			}
			this.OnInteracted.Send(this);
		}
	}

	public void ForceEnter(Quaternion rotationX, Quaternion rotationY, int ladderSide, Action callback = null, bool isLadder = true)
	{
		base.StartCoroutine(this.InternalForceEnter(rotationX, rotationY, ladderSide, callback, isLadder));
	}

	private IEnumerator InternalForceEnter(Quaternion rotationX, Quaternion rotationY, int ladderSide, Action callback, bool isLadder)
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		this.RemoveListeners();
		if (this.m_HideCrosshair)
		{
			GameManager.Instance.HideCrosshair();
		}
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.Player.HideFirstPersonArms();
		this.m_IsInteracted = true;
		if (isLadder)
		{
			GameManager.Instance.Player.AnimatorBody.SetInteger("LadderSide", ladderSide);
		}
		GameManager.Instance.Player.EnterInteractionInstant(this.m_AnimationTrigger + "Instant");
		Debug.Log("EnterInteractionInstant: " + this.m_AnimationTrigger + "Instant");
		for (int i = 0; i < this.m_InteractableEnterAnimators.Length; i = unchecked(i + 1))
		{
			InteractableAnimator interactableAnimator;
			interactableAnimator = this.m_InteractableEnterAnimators[i];
			if (interactableAnimator.DisableOnComplete)
			{
				interactableAnimator.Animator.gameObject.SetActive(value: false);
			}
		}
		if (this.m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y);
		}
		GameManager.Instance.Player.ForceRotation(rotationX, rotationY);
		if (!this.CanCancel && !this.HasEndLocation)
		{
			if (!this.m_IsSingleInteraction)
			{
				this.AddListeners();
			}
		}
		else
		{
			this.m_IsInteracted = true;
		}
		if (this.m_IsSingleInteraction)
		{
			this.m_Interactable.SetActive(active: false);
		}
		callback?.Invoke();
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
		for (int i = 0; i < this.m_InteractableEnterAnimators.Length; i = unchecked(i + 1))
		{
			InteractableAnimator interactableAnimator;
			interactableAnimator = this.m_InteractableEnterAnimators[i];
			if (interactableAnimator.DisableOnComplete)
			{
				interactableAnimator.Animator.gameObject.SetActive(value: false);
			}
		}
		if (this.m_EnableAnimationRotation)
		{
			GameManager.Instance.Player.EnableAnimationRotation(this.m_RotationLock.x, this.m_RotationLock.y);
		}
		if (!this.CanCancel && !this.HasEndLocation)
		{
			if (!this.m_IsSingleInteraction)
			{
				this.AddListeners();
			}
		}
		else
		{
			this.m_IsInteracted = true;
		}
		this.OnEnter.Send(this);
		if (this.m_IsSingleInteraction)
		{
			this.m_Interactable.SetActive(active: false);
		}
		if (this.m_IsSingleInteractionExit)
		{
			this.m_IsInteracted = false;
			GameManager.Instance.Player.ExitAnimation();
			GameManager.Instance.Player.SetBodyAnimationTrigger(this.m_AnimationExitTrigger);
			if (!this.m_IsSingleInteraction)
			{
				this.AddListeners();
			}
			if (this.m_HideCrosshair)
			{
				GameManager.Instance.ShowCrosshair(this.m_IsReal);
			}
			if (this.HasEndLocation && this.m_AnimationExitTrigger == "")
			{
				Vector3 vector;
				vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
				GameManager.Instance.Player.transform.position = this.m_EndLocation.position + vector;
				GameManager.Instance.Player.transform.eulerAngles = this.m_EndLocation.eulerAngles;
			}
			this.OnExit.Send(this);
		}
	}

	private void Cancel()
	{
		this.Exit();
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.m_CancelLocation.eulerAngles, 0.1f).SetEase(Ease.Linear));
		this.m_Sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.m_CancelLocation.position + vector, 0.1f).SetEase(Ease.Linear));
	}

	public void Exit()
	{
		this.m_IsInteracted = false;
		for (int i = 0; i < this.m_InteractableExitAnimators.Length; i = unchecked(i + 1))
		{
			this.m_InteractableExitAnimators[i].Animator.SetTrigger(this.m_InteractableExitAnimators[i].Trigger);
		}
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		if (this.m_EnableAnimationRotation)
		{
			this.ResetSequence();
			this.m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		if (this.HasEndLocation)
		{
			Vector3 vector;
			vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = this.m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = this.m_EndLocation.eulerAngles;
		}
		if (!this.m_IsSingleInteraction)
		{
			this.m_Interactable.ResetAction();
			this.SetActive(active: true);
		}
		this.OnExit.Send(this);
		if (this.m_HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair(this.m_IsReal);
		}
		GameManager.Instance.Player.ExitAnimation();
	}

	public void SendOnExit()
	{
		this.OnExit.Send(this);
	}

	public void Stop()
	{
		this.m_IsInteracted = false;
		this.KillSequence();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		}
	}

	private void AddListeners()
	{
		if (this.m_Interactable != null)
		{
			this.m_Interactable.ResetAction();
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		}
	}

	private void RemoveListeners()
	{
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		}
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
		this.m_Interactable.OnEnter -= this.HandleInteractableOnEnter;
		this.m_Interactable.OnExit -= this.HandleInteractableOnExit;
		this.RemoveListeners();
		this.KillSequence();
		base.OnDisposed();
	}
}
