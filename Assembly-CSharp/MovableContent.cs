using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class MovableContent : ActionEventContent<MovableContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public MovableEnter EnterFront;

		public MovableEnter EnterBack;
	}

	[Header("S13Content")]
	[SerializeField]
	private S13ActionGroupReference onStepAction;

	[SerializeField]
	private S13ActionGroupReference onReleaseCartAction;

	[SerializeField]
	public ControllerRumble.RUMBLE_PRESETS m_presetRumble = ControllerRumble.RUMBLE_PRESETS.Cart_Drag;

	private Movable m_Movable;

	private MovableLocation[] m_MovableLocations;

	private MovableLocation m_MovableStartLocation;

	private MovableEnter m_InteractedEnter;

	private Sequence m_Sequence;

	private bool m_CanMove;

	private bool m_IsMoving;

	private bool m_IsForward = true;

	public event EventHandler OnExit;

	public event EventHandler OnFrontEnter;

	public event EventHandler OnBackEnter;

	protected override void OnInitialize()
	{
		this.m_Movable = (Movable)base.Connectable;
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.OnEnter -= this.HandleFrontOnEnter;
				properties.EnterFront.OnEnter += this.HandleFrontOnEnter;
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.OnEnter -= this.HandleBackOnEnter;
				properties.EnterBack.OnEnter += this.HandleBackOnEnter;
			}
		}
	}

	public void SetStartLocation(MovableLocation[] path, MovableLocation startLocation)
	{
		this.m_MovableLocations = path;
		this.m_MovableStartLocation = startLocation;
		for (int i = 0; i < this.m_MovableLocations.Length; i = unchecked(i + 1))
		{
			if (this.m_MovableLocations[i] == this.m_MovableStartLocation)
			{
				this.m_Movable.Data.SetCurrentIndex(i);
				break;
			}
		}
		base.transform.position = this.m_MovableLocations[this.m_Movable.Data.CurrentIndex].transform.position;
		base.transform.eulerAngles = this.m_MovableLocations[this.m_Movable.Data.CurrentIndex].transform.eulerAngles;
	}

	protected override void InternalEnable()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.gameObject.SetActive(value: true);
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.gameObject.SetActive(value: true);
			}
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.gameObject.SetActive(value: false);
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (!this.m_CanMove || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!this.m_IsMoving)
		{
			if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
			{
				this.Exit();
			}
			else if (PlayerInput.InteractOnReleased())
			{
				this.Exit();
			}
			else if (PlayerInput.MoveY() > 0f)
			{
				this.MoveForward();
			}
			else if (PlayerInput.MoveY() < 0f)
			{
				this.MoveBack();
			}
			if (GameManager.Instance.Player.BattleStatus == BattleStatus.Moving)
			{
				GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
			}
		}
		else if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.Moving);
		}
	}

	private void Exit()
	{
		this.m_CanMove = false;
		GameManager.Instance.Player.transform.SetParent(null);
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandleExitOnAnimationComplete;
		GameManager.Instance.Player.ExitInteraction();
		this.ResetSequence();
		this.m_Sequence.Insert(0f, GameManager.Instance.Player.AnimationContainer.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
		this.onReleaseCartAction?.Execute();
	}

	private void MoveForward()
	{
		if (this.m_IsForward)
		{
			if (this.m_Movable.Data.CurrentIndex >= unchecked(this.m_MovableLocations.Length - 1))
			{
				return;
			}
			this.CheckForward();
		}
		else
		{
			if (this.m_Movable.Data.CurrentIndex <= 0)
			{
				return;
			}
			this.CheckBack();
		}
		this.DOMove();
		this.onStepAction?.Execute();
	}

	private void MoveBack()
	{
		if (this.m_IsForward)
		{
			if (this.m_Movable.Data.CurrentIndex <= 0)
			{
				return;
			}
			this.CheckBack();
		}
		else
		{
			if (this.m_Movable.Data.CurrentIndex >= unchecked(this.m_MovableLocations.Length - 1))
			{
				return;
			}
			this.CheckForward();
		}
		this.DOMove(isForward: false);
		this.onStepAction?.Execute();
	}

	private void CheckForward()
	{
		this.m_IsMoving = true;
		this.m_Movable.Data.SetCurrentIndex(unchecked(this.m_Movable.Data.CurrentIndex + 1));
	}

	private void CheckBack()
	{
		this.m_IsMoving = true;
		this.m_Movable.Data.SetCurrentIndex(unchecked(this.m_Movable.Data.CurrentIndex - 1));
	}

	private void DOMove(bool isForward = true)
	{
		GameManager.Instance.Player.SetAnimationTrigger(isForward ? "CartForward" : "CartBack");
		base.transform.DOKill();
		base.transform.DORotate(this.m_MovableLocations[this.m_Movable.Data.CurrentIndex].transform.eulerAngles, 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
		base.transform.DOMove(this.m_MovableLocations[this.m_Movable.Data.CurrentIndex].transform.position, 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
			.OnComplete(this.MoveOnComplete);
		if (this.m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[unchecked((int)this.m_presetRumble)]);
		}
	}

	private void HandleFrontOnEnter(object sender, EventArgs e)
	{
		this.Enter((MovableEnter)sender);
		this.OnFrontEnter.Send(this);
	}

	private void HandleBackOnEnter(object sender, EventArgs e)
	{
		this.Enter((MovableEnter)sender, isFront: false);
		this.OnBackEnter.Send(this);
	}

	private void Enter(MovableEnter interacted, bool isFront = true)
	{
		this.m_InteractedEnter = interacted;
		this.m_CanMove = true;
		this.m_IsForward = isFront;
		GameManager.Instance.Player.transform.SetParent(base.transform);
		GameManager.Instance.Player.S13SetCartMove(isMoving: true);
	}

	public void ForceEnterFront(MovableDataObject dataObject)
	{
		this.PrepForceEnter();
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterFront != null && dataObject.Side == 0)
			{
				properties.EnterFront.ForceEnter(dataObject.RotationX, dataObject.RotationY, dataObject.Side, () =>
				{
					this.ForceEnter(properties.EnterFront, isFront: true);
				}, isLadder: false);
				break;
			}
		}
	}

	public void ForceEnterBack(MovableDataObject dataObject)
	{
		this.PrepForceEnter();
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterBack != null && dataObject.Side == 1)
			{
				properties.EnterBack.ForceEnter(dataObject.RotationX, dataObject.RotationY, dataObject.Side, () =>
				{
					this.ForceEnter(properties.EnterBack, isFront: false);
				}, isLadder: false);
				break;
			}
		}
	}

	private void PrepForceEnter()
	{
		this.SetStartLocation(this.m_Movable.Path.Path, this.m_Movable.Path.Path[this.m_Movable.Data.CurrentIndex]);
		GameManager.Instance.Player.transform.SetParent(base.transform);
	}

	private void ForceEnter(MovableEnter interacted, bool isFront)
	{
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		this.m_InteractedEnter = interacted;
		this.m_CanMove = true;
		this.m_IsForward = isFront;
		GameManager.Instance.Player.S13SetCartMove(isMoving: true);
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		if (!this.m_InteractedEnter.IsSingleInteraction)
		{
			this.m_InteractedEnter.Interactable?.ResetAction();
			this.m_InteractedEnter.SetActive(active: true);
		}
		this.m_InteractedEnter.SendOnExit();
		this.OnExit.Send(this);
		if (this.m_InteractedEnter.HideCrosshair)
		{
			GameManager.Instance.ShowCrosshair();
		}
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.S13SetCartMove(isMoving: false);
	}

	private void MoveOnComplete()
	{
		this.m_IsMoving = false;
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
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		}
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.EnterFront != null)
			{
				properties.EnterFront.OnEnter -= this.HandleFrontOnEnter;
			}
			if (properties.EnterBack != null)
			{
				properties.EnterBack.OnEnter -= this.HandleBackOnEnter;
			}
		}
		this.OnExit = null;
		this.OnFrontEnter = null;
		this.OnBackEnter = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
