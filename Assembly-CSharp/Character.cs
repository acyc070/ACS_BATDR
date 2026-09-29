using System;
using DG.Tweening;
using S13Audio.BATDR;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public abstract class Character : CharacterStateMachine
{
	[SerializeField]
	private CharacterMovement m_Movement;

	[Header("TEST")]
	[SerializeField]
	private bool m_IsStopAndGo;

	[SerializeField]
	private Transform m_WeaponParent;

	[SerializeField]
	private CharacterActionVision m_CharacterVision;

	[SerializeField]
	private CharacterActionGroup m_ActionGroupAttack;

	[SerializeField]
	private CharacterActionGroup m_ActionGroupEvade;

	protected CharacterAction m_ActiveAction;

	private bool m_DisableVision;

	private bool m_CanAttack = true;

	private Sequence m_SlideSequence;

	protected float m_MoveSpeed = 0.5f;

	protected float m_MoveXSpeed;

	protected bool m_RunInput;

	public bool IsAlerted;

	[HideInInspector]
	public bool IsAttackDelay;

	public abstract bool JumpInput { get; }

	public abstract bool CrouchInput { get; }

	public abstract bool RunInput { get; }

	public abstract float MoveXInput { get; }

	public abstract float MoveYInput { get; }

	public abstract float RotateXInput { get; }

	public abstract bool AttackInput { get; }

	public CharacterMovement Movement => this.m_Movement;

	public bool IsStopAndGo => this.m_IsStopAndGo;

	public Transform WeaponParent => this.m_WeaponParent;

	public CharacterActionVision CharacterVision => this.m_CharacterVision;

	public CharacterActionGroup ActionGroupAttack => this.m_ActionGroupAttack;

	public CharacterActionGroup ActionGroupEvade => this.m_ActionGroupEvade;

	public CharacterAction ActiveAction => this.m_ActiveAction;

	public CharacterContent Content { get; private set; }

	public CharacterController Controller { get; private set; }

	public CharacterRotation Rotation { get; private set; }

	public CharacterAgent Agent { get; private set; }

	public CharacterNode CurrentNode { get; private set; }

	public CharacterNode PreviousNode { get; private set; }

	public Transform Target { get; private set; }

	public bool DisableVision => this.m_DisableVision;

	public bool CanAttack => this.m_CanAttack;

	public bool InCombat
	{
		get
		{
			if (GameManager.Instance.Player == null)
			{
				return false;
			}
			return this.Target == GameManager.Instance.Player.transform;
		}
	}

	public BATDRNpcAudioController AudioController { get; private set; }

	public Vector3 VisionPosition
	{
		get
		{
			if (this.Content.EyeSight != null)
			{
				return this.Content.EyeSight.position;
			}
			return base.transform.position + Vector3.up * (this.Agent.Agent.height / 2f);
		}
	}

	public event EventHandler OnInitializeOnComplete;

	public event EventHandler OnAnimationEvent;

	public event EventHandler OnAnimationEvent2;

	public event EventHandler OnAnimationEnter;

	public event EventHandler OnAnimationComplete;

	public event EventHandler OnNodeReached;

	public event EventHandler OnDamageTaken;

	public event EventHandler OnTargetLost;

	public event EventHandler OnCharacterDeath;

	public void SetCharacterAction(CharacterAction action)
	{
		this.m_ActiveAction = action;
	}

	public void SetMoveSpeed(float speed)
	{
		this.m_MoveSpeed = speed;
	}

	public void SetMoveXSpeed(float speed)
	{
		this.m_MoveXSpeed = speed;
	}

	public void SetRun(bool isRunning)
	{
		this.m_RunInput = isRunning;
	}

	public void SetTarget(Transform target)
	{
		this.Target = target;
	}

	public void SetDisableVision(bool active)
	{
		this.m_DisableVision = active;
	}

	public void SetCanAttack(bool active)
	{
		this.m_CanAttack = active;
	}

	protected override void InternalInitializeOnComplete()
	{
		this.GetCharacterContent();
		this.SetupCharacterContent();
		base.SetState(State.Character.Idle);
		this.GetAudioController();
		this.CharacterInitialized();
		this.SendOnInitializeOnComplete();
	}

	protected virtual void CharacterInitialized()
	{
	}

	protected void GetCharacterContent()
	{
		this.Content = null;
		this.SetContent(base.gameObject.GetComponentInChildren<CharacterContent>());
	}

	protected void SetContent(CharacterContent content)
	{
		if (!(content == null))
		{
			this.Content = content;
			this.Content.GenericAnimationEvents.SetReciever(this);
			this.Content.Initialize();
		}
	}

	protected void SetupCharacterContent()
	{
		if (this.Controller == null)
		{
			this.Controller = base.gameObject.GetComponent<CharacterController>();
		}
		this.m_Movement.Initialize(this);
		if (this.Rotation == null)
		{
			this.Rotation = new CharacterRotation(this);
		}
		if (this.Agent == null)
		{
			this.Agent = new CharacterAgent(this);
		}
		if (this.Agent != null)
		{
			this.Agent.ResetAgent();
		}
	}

	protected void GetAudioController()
	{
		if (this.AudioController == null)
		{
			this.AudioController = base.gameObject.GetComponentInChildren<BATDRNpcAudioController>();
		}
	}

	public virtual void OnInteract()
	{
	}

	public virtual void OnAttack()
	{
	}

	public virtual void OnAttackRanged()
	{
	}

	public virtual void OnHit(RaycastHit hit)
	{
	}

	public virtual void OnDeath()
	{
	}

	public void SendOnAnimationEnter()
	{
		this.OnAnimationEnter.Send(this);
	}

	public void AnimationEvent()
	{
		this.OnAnimationEvent.Send(this);
	}

	public void AnimationEvent2()
	{
		this.OnAnimationEvent2.Send(this);
	}

	public void AnimationEnter()
	{
		this.OnAnimationEnter.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	public void CharacterDeath()
	{
		GameManager.Instance.Player.RemoveEnemy(this);
		this.OnCharacterDeath.Send(this);
	}

	public void SetNode(CharacterNode node)
	{
		if (this.CurrentNode != null)
		{
			this.CurrentNode.OnReached -= this.HandleNodeOnReached;
		}
		this.PreviousNode = this.CurrentNode;
		if (node != null)
		{
			this.CurrentNode = node;
			this.InternalSetNode(this.CurrentNode);
			this.CurrentNode.OnReached -= this.HandleNodeOnReached;
			this.CurrentNode.OnReached += this.HandleNodeOnReached;
			this.SetTarget(this.CurrentNode.transform);
		}
		else
		{
			this.SetTarget(null);
		}
	}

	protected virtual void InternalSetNode(CharacterNode node)
	{
	}

	private void HandleNodeOnReached(object sender, EventArgs e)
	{
		CharacterNode characterNode;
		characterNode = sender as CharacterNode;
		characterNode.OnReached -= this.HandleNodeOnReached;
		this.Rotation?.ForceRotation(characterNode.transform.rotation);
		this.InternalHandleNodeOnReached(characterNode);
		this.SlideTo(characterNode.transform);
		this.SendOnNodeReached();
	}

	protected virtual void InternalHandleNodeOnReached(CharacterNode node)
	{
	}

	public void ForceNodeOnReached(float duration = 0.35f)
	{
		if (!(this.CurrentNode == null))
		{
			this.Rotation?.ForceRotation(this.CurrentNode.transform.rotation);
			this.InternalHandleNodeOnReached(this.CurrentNode);
			this.SlideTo(this.CurrentNode.transform, duration);
			this.SendOnNodeReached();
		}
	}

	public void SlideTo(Transform target, float duration = 0.35f, Action onComplete = null)
	{
		this.m_SlideSequence?.Kill();
		this.m_SlideSequence = DOTween.Sequence();
		this.m_SlideSequence.SetUpdate(UpdateType.Fixed);
		this.m_SlideSequence.Insert(0f, base.transform.DOMove(target.position, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		this.m_SlideSequence.Insert(0f, base.transform.DORotate(target.eulerAngles, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		this.m_SlideSequence.OnComplete(() =>
		{
			onComplete?.Invoke();
		});
	}

	public void CancelSlide()
	{
		this.m_SlideSequence?.Kill();
		this.m_SlideSequence = null;
	}

	public void ClearAnimationTriggers()
	{
		if (this.Content != null && this.Content.Animator != null)
		{
			this.Content.Animator.ResetTrigger("Attack");
			this.Content.Animator.ResetTrigger("AttackBehind");
			this.Content.Animator.ResetTrigger("AttackLeap");
			this.Content.Animator.ResetTrigger("AttackRanged");
		}
	}

	public void CancelPath()
	{
		if (this.Agent != null)
		{
			this.Agent.CancelPath();
		}
	}

	public void ForceStop(bool smooth = true)
	{
		this.CancelPath();
		this.SetRun(isRunning: false);
		if (this.Content != null && this.Content.Animator != null)
		{
			this.Content.Animator.SetMovementState(0f, smooth);
			this.Content.Animator.SetMovementSpeed(0f, smooth);
		}
	}

	public void ForceRotation(Quaternion quaternion)
	{
		this.Rotation.ForceRotation(quaternion);
		this.Rotation.Update();
	}

	protected void SendOnInitializeOnComplete()
	{
		this.OnInitializeOnComplete.Send(this);
	}

	public void SendOnNodeReached()
	{
		this.OnNodeReached.Send(this);
	}

	public void SendOnTargetLost()
	{
		this.OnTargetLost.Send(this);
	}

	public void SendOnDamageTaken()
	{
		this.OnDamageTaken.Send(this);
	}

	protected override void OnDisposed()
	{
		this.m_SlideSequence?.Kill();
		this.m_SlideSequence = null;
		this.OnInitializeOnComplete = null;
		this.OnAnimationEvent = null;
		this.OnAnimationEvent2 = null;
		this.OnAnimationEnter = null;
		this.OnAnimationComplete = null;
		this.OnNodeReached = null;
		this.OnDamageTaken = null;
		this.OnTargetLost = null;
		this.OnCharacterDeath = null;
		this.Content = null;
		this.Controller = null;
		base.OnDisposed();
	}
}
