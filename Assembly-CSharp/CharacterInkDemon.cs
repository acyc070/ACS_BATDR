using System;
using DG.Tweening;
using UnityEngine;

public class CharacterInkDemon : Character
{
	[SerializeField]
	private CharacterContent m_CharacterInkDemon;

	private bool isDead;

	private HeadTracker m_HeadTracker;

	private Sequencer m_PlayerSequencer;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => false;

	public override float MoveXInput => 0f;

	public override float MoveYInput => 1f;

	public override float RotateXInput => 0f;

	public override bool AttackInput => false;

	public event EventHandler OnEnterPortal;

	public void ExitPortal()
	{
		base.SetState(State.Character.Cutscene);
		this.m_CharacterInkDemon.Animator.SetInteger("SpecialType", 0);
		this.m_CharacterInkDemon.Animator.SetTrigger("Special");
	}

	public void OnExitComplete()
	{
		base.Agent.Agent.enabled = false;
		base.Agent.Agent.enabled = true;
		GameObject.Find("AiManager")?.GetComponent<AiManager>();
		base.SetTarget(GameManager.Instance.Player.transform);
		base.SetState(State.Character.Follow);
	}

	public void EnterPortal()
	{
		base.SetState(State.Character.Cutscene);
		this.m_CharacterInkDemon.Animator.SetInteger("SpecialType", 1);
		this.m_CharacterInkDemon.Animator.SetTrigger("Special");
	}

	public void OnEnterComplete()
	{
		base.Dispose();
	}

	protected override void InternalUpdate()
	{
		base.InternalUpdate();
		if (this.isDead || base.CurrentState != State.Character.Follow)
		{
			return;
		}
		Player player;
		player = GameManager.Instance.Player;
		if (base.Target == player.transform)
		{
			if (Vector3.Distance(base.transform.position, player.transform.position) < 5f)
			{
				this.isDead = true;
				this.m_HeadTracker.SetActive(active: false);
				base.SetState(State.Character.Cutscene);
				this.m_CharacterInkDemon.Animator.SetTrigger("Death");
				player.Death();
				this.m_PlayerSequencer.CreateSequence(player.transform.DOMove(base.transform.position + base.transform.forward * 2.5f, 0.25f).SetEase(Ease.InOutSine), player.transform.DORotate(base.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.25f).SetEase(Ease.InOutSine));
			}
		}
		else if (base.Content.Animator.GetFloat("MovementState") == 0f)
		{
			this.OnEnterPortal.Send(this);
			base.SetState(State.Character.Cutscene);
		}
	}

	protected override void InternalInitialize()
	{
		base.SetContent(this.m_CharacterInkDemon);
	}

	protected override void InternalInitializeOnComplete()
	{
		this.m_PlayerSequencer = new Sequencer();
		base.SetState(State.Character.Cutscene);
		Transform transform;
		transform = base.transform.FindDeepChild("BN_Head");
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head001");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head_01");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("Head_Ctrl");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("CTRL_Mouth");
		}
		if (transform != null)
		{
			this.m_HeadTracker = transform.gameObject.AddComponent<HeadTracker>();
			Transform transform2;
			transform2 = new GameObject().transform;
			transform2.SetParent(base.transform);
			transform2.localPosition = Vector3.forward;
			transform2.localEulerAngles = Vector3.zero;
			this.m_HeadTracker.SetForwardDirection(transform2);
			this.m_HeadTracker.SetOffset(new Vector3(0f, 270f, 260f));
			this.m_HeadTracker.SetAngle(75f);
		}
		base.Agent.Agent.enabled = false;
		base.Agent.Agent.enabled = true;
	}

	protected override void OnDisposed()
	{
		this.m_PlayerSequencer?.Kill();
		this.m_PlayerSequencer = null;
		base.OnDisposed();
	}
}
