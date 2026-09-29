using System;
using DG.Tweening;
using UnityEngine;

public class LadderContent : ActionEventContent<LadderContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private const int LADDER_MINIMUM = 3;

	private const float LADDER_HEIGHT = 1f;

	private const float LADDER_MOVE_SPEED = 0.333f;

	private LadderPart[] m_Ladders;

	private LadderBottom m_LadderBottom;

	private LadderTop m_LadderTop;

	private Ease m_TweenEase = Ease.Linear;

	private UpdateType m_TweenUpdateType = UpdateType.Fixed;

	private int m_LadderIndex;

	private int m_CurrentLadderIndex;

	private int m_LadderSide;

	private int m_ClimbDirection;

	public LadderBottom LadderBottom => this.m_LadderBottom;

	public LadderTop LadderTop => this.m_LadderTop;

	public int LadderIndex => this.m_LadderIndex;

	public int CurrentLadderIndex => this.m_CurrentLadderIndex;

	public int LadderSide => this.m_LadderSide;

	public int ClimbDirection => this.m_ClimbDirection;

	public bool IsActive { get; private set; }

	public bool IsMoving { get; private set; }

	public event EventHandler OnDeath;

	protected override void OnInitialize()
	{
		this.IsActive = false;
		this.IsMoving = false;
		this.m_LadderIndex = 3;
		this.m_Ladders = base.GetComponentsInChildren<LadderPart>();
		this.m_LadderBottom = base.GetComponentInChildren<LadderBottom>();
		this.m_LadderTop = base.GetComponentInChildren<LadderTop>();
		unchecked
		{
			for (int i = 0; i < this.m_Ladders.Length; i++)
			{
				this.m_LadderIndex += 2;
			}
			this.LadderAddListeners();
		}
	}

	private void Update()
	{
		if (!this.IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.IsMoving)
		{
			if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
			{
				GameManager.Instance.Player.SetBattleStatus(BattleStatus.Moving);
			}
			return;
		}
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.Moving)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
		float num;
		num = PlayerInput.MoveY();
		if (num != 0f)
		{
			this.IsMoving = true;
		}
		if (num > 0f)
		{
			if (this.m_CurrentLadderIndex >= this.m_LadderIndex)
			{
				this.ExitLadder(this.m_LadderTop);
			}
			else
			{
				this.DoMoveUp();
			}
		}
		else if (num < 0f)
		{
			if (this.m_CurrentLadderIndex <= 0)
			{
				this.ExitLadder(this.m_LadderBottom);
			}
			else
			{
				this.DoMoveDown();
			}
		}
	}

	private void EnterLadder(int startingIndex = 0)
	{
		if (startingIndex == 0)
		{
			this.m_LadderSide = 0;
			this.m_ClimbDirection = 1;
		}
		else
		{
			this.m_LadderSide = 1;
			this.m_ClimbDirection = 2;
		}
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		this.m_CurrentLadderIndex = startingIndex;
		this.LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += this.HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += this.HandlePlayerOnForceDeath;
		GameManager.Instance.Player.S13SetClimbing(isClimbing: true);
	}

	private void EnterLadderOnComplete()
	{
		this.LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += this.HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += this.HandlePlayerOnForceDeath;
		this.IsActive = true;
	}

	public void ForceEnterBottom(LadderDataObject dataObject)
	{
		this.PrepareForceEnter(dataObject);
		this.m_LadderBottom.ForceEnter(dataObject.RotationX, dataObject.RotationY, this.m_LadderSide, this.ForceEnter);
	}

	public void ForceEnterTop(LadderDataObject dataObject)
	{
		this.PrepareForceEnter(dataObject);
		this.m_LadderTop.ForceEnter(dataObject.RotationX, dataObject.RotationY, this.m_LadderSide, this.ForceEnter);
	}

	private void PrepareForceEnter(LadderDataObject dataObject)
	{
		this.m_ClimbDirection = dataObject.ClimbDirection;
		this.m_LadderSide = dataObject.LadderSide;
		this.m_CurrentLadderIndex = dataObject.LadderIndex;
	}

	private void ForceEnter()
	{
		GameManager.Instance.HideInteraction();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		Vector3 position;
		position = this.m_LadderBottom.StartLocation.position;
		float num;
		num = 1f * (float)this.m_CurrentLadderIndex;
		if (this.m_CurrentLadderIndex > 0)
		{
			position.y += num;
		}
		GameManager.Instance.Player.transform.position = position;
		this.LadderRemoveListeners();
		GameManager.Instance.Player.OnAnimationComplete += this.HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnForceDeath += this.HandlePlayerOnForceDeath;
		GameManager.Instance.Player.S13SetClimbing(isClimbing: true);
		this.IsActive = true;
	}

	private void DoMoveUp()
	{
		unchecked
		{
			this.m_CurrentLadderIndex++;
			this.DoMove("LadderMoveUp", GameManager.Instance.Player.transform.position.y + 1f);
		}
	}

	private void DoMoveDown()
	{
		unchecked
		{
			this.m_CurrentLadderIndex--;
			this.DoMove("LadderMoveDown", GameManager.Instance.Player.transform.position.y - 1f);
		}
	}

	private void DoMove(string trigger, float positionY)
	{
		if (this.IsActive)
		{
			if (this.m_LadderSide == 1)
			{
				this.m_LadderSide = 2;
			}
			else
			{
				this.m_LadderSide = 1;
			}
			this.ResetTriggers();
			GameManager.Instance.Player.SetAnimationTrigger(trigger);
			GameManager.Instance.Player.transform.DOKill();
			GameManager.Instance.Player.transform.DOMoveY(positionY, 0.333f).SetEase(this.m_TweenEase).SetUpdate(this.m_TweenUpdateType)
				.OnComplete(this.EnableMove);
		}
	}

	private void ExitLadder(InteractableAnimation exitLadder)
	{
		this.IsActive = false;
		this.IsMoving = false;
		this.m_LadderBottom.ResetAll();
		this.m_LadderTop.ResetAll();
		exitLadder.OnExit -= this.HandleExitLadderOnExit;
		exitLadder.OnExit += this.HandleExitLadderOnExit;
		exitLadder.Exit();
		this.LadderRemoveListeners();
		GameManager.Instance.Player.OnForceDeath += this.HandlePlayerOnForceDeath;
		this.ResetTriggers();
		this.LadderAddListeners();
	}

	private void HandleExitLadderOnExit(object sender, EventArgs e)
	{
		(sender as InteractableAnimation).OnExit -= this.HandleExitLadderOnExit;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandlePlayerOnAnimationComplete;
			GameManager.Instance.Player.OnForceDeath -= this.HandlePlayerOnForceDeath;
			if (GameManager.Instance.Player.HasTeleport() && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				GameManager.Instance.EnableTeleport();
			}
			GameManager.Instance.Player.S13SetClimbing(isClimbing: false);
		}
	}

	private void ResetTriggers()
	{
		GameManager.Instance.Player.ResetAnimationTrigger("LadderEnterBottom");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderEnterTop");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderMoveUp");
		GameManager.Instance.Player.ResetAnimationTrigger("LadderMoveDown");
	}

	private void EnableMove()
	{
		this.IsMoving = false;
	}

	private void HandleLadderTopOnInteracted(object sender, EventArgs e)
	{
		this.EnterLadder(this.m_LadderIndex);
	}

	private void HandleLadderBottomOnInteracted(object sender, EventArgs e)
	{
		this.EnterLadder();
	}

	private void HandlePlayerOnAnimationComplete(object sender, EventArgs e)
	{
		this.EnterLadderOnComplete();
	}

	private void HandlePlayerOnForceDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnForceDeath -= this.HandlePlayerOnForceDeath;
		GameManager.Instance.Player.transform.DOKill();
		this.IsActive = false;
		this.IsMoving = false;
		this.m_LadderBottom.Stop();
		this.m_LadderTop.Stop();
		this.m_LadderBottom.ResetAll();
		this.m_LadderTop.ResetAll();
		this.m_LadderBottom.OnExit -= this.HandleExitLadderOnExit;
		this.m_LadderTop.OnExit -= this.HandleExitLadderOnExit;
		this.LadderRemoveListeners();
		this.ResetTriggers();
		this.LadderAddListeners();
		if (GameManager.Instance.Player.HasTeleport() && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
		{
			GameManager.Instance.EnableTeleport();
		}
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.S13SetClimbing(isClimbing: false);
		this.OnDeath.Send(this);
	}

	protected override void InternalEnable()
	{
		this.SetLadderActive(active: true);
		ActiveSetter[] componentsInChildren;
		componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].SetActive(active: true);
		}
	}

	protected override void InternalDisable()
	{
		this.SetLadderActive(active: false);
	}

	private void SetLadderActive(bool active)
	{
		this.IsActive = false;
		this.IsMoving = false;
		this.m_LadderBottom.SetActive(active);
		this.m_LadderTop.SetActive(active);
		if (active)
		{
			this.LadderRemoveListeners();
			this.LadderAddListeners();
		}
		else
		{
			this.LadderRemoveListeners();
		}
	}

	protected override void OnForceActivateComplete()
	{
		this.Enable();
	}

	private void LadderAddListeners()
	{
		if (this.m_LadderBottom != null)
		{
			this.m_LadderBottom.OnInteracted += this.HandleLadderBottomOnInteracted;
		}
		if (this.m_LadderTop != null)
		{
			this.m_LadderTop.OnInteracted += this.HandleLadderTopOnInteracted;
		}
	}

	private void LadderRemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandlePlayerOnAnimationComplete;
			GameManager.Instance.Player.OnForceDeath -= this.HandlePlayerOnForceDeath;
		}
		if (this.m_LadderBottom != null)
		{
			this.m_LadderBottom.OnInteracted -= this.HandleLadderBottomOnInteracted;
		}
		if (this.m_LadderTop != null)
		{
			this.m_LadderTop.OnInteracted -= this.HandleLadderTopOnInteracted;
		}
	}

	protected override void OnDisposed()
	{
		this.LadderRemoveListeners();
		this.m_Ladders = null;
		this.m_LadderTop = null;
		this.m_LadderBottom = null;
		this.OnDeath = null;
		base.OnDisposed();
	}
}
