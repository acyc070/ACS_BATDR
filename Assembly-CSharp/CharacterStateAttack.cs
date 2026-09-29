using System;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateAttack(Character character, State.Character state) : CharacterState(character, state)
{
	private CharacterActionAttack m_Action;

	private float m_AttackType;

	private float m_LastAttackType;

	private float[] m_AttackTypes = new float[3] { 0f, 1f, 2f };

	private bool m_IsMoving;

	public override void InternalOnStateEnter()
	{
		base.Actor.ForceStop();
		base.Actor.SetMoveSpeed(0f);
		base.Actor.SetMoveXSpeed(0f);
		this.m_IsMoving = false;
		this.m_Action = base.Actor.ActiveAction as CharacterActionAttack;
		this.GetAttackType();
		this.RemoveListeners();
		this.AddListeners();
		string animationTrigger;
		animationTrigger = (this.m_Action ? this.m_Action.AnimationTrigger : "Attack");
		base.Actor.Content?.SetAnimationTrigger(animationTrigger);
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Attack);
	}

	protected override void InternalUpdate()
	{
		if (this.m_IsMoving)
		{
			base.Actor.Movement?.UpdateInput();
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (this.m_IsMoving)
		{
			base.Actor.Movement?.Update();
		}
	}

	private void HandleActorOnAnimationEnter(object sender, EventArgs e)
	{
		base.Actor.OnAnimationEnter -= this.HandleActorOnAnimationEnter;
		Vector3 vector;
		vector = (this.m_Action ? this.m_Action.StartRotation : Vector3.zero);
		base.Actor.ForceRotation(Quaternion.Euler(base.Actor.transform.eulerAngles + vector));
		float num;
		num = (this.m_Action ? this.m_Action.MoveSpeed : 0f);
		if (num != 0f)
		{
			base.Actor.SetRun(isRunning: true);
			base.Actor.SetMoveSpeed(num);
			this.m_IsMoving = true;
		}
	}

	private void HandleActorOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= this.HandleActorOnAnimationComplete;
		this.m_IsMoving = false;
		base.Actor.ForceStop(smooth: false);
	}

	public override void InternalOnStateExit()
	{
		base.Actor.IsAttackDelay = true;
		this.RemoveListeners();
		base.Actor.SetMoveSpeed(base.Actor.Movement.MoveSpeed);
		base.Actor.SetMoveXSpeed(0f);
		base.Actor.ForceStop(smooth: false);
		this.m_Action = null;
	}

	private void GetAttackType()
	{
		while (this.m_AttackType == this.m_LastAttackType)
		{
			this.m_AttackType = this.m_AttackTypes[global::UnityEngine.Random.Range(0, this.m_AttackTypes.Length)];
		}
		this.m_LastAttackType = this.m_AttackType;
		base.Actor.Content.Animator.SetAttackType(this.m_AttackType);
	}

	private void AddListeners()
	{
		base.Actor.OnAnimationEnter += this.HandleActorOnAnimationEnter;
		base.Actor.OnAnimationComplete += this.HandleActorOnAnimationComplete;
	}

	private void RemoveListeners()
	{
		base.Actor.OnAnimationEnter -= this.HandleActorOnAnimationEnter;
		base.Actor.OnAnimationComplete -= this.HandleActorOnAnimationComplete;
	}
}
