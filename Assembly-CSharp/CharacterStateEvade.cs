using System;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateEvade : CharacterState
{
	private CharacterActionEvade m_Action;

	private bool m_IsMoving;

	public CharacterStateEvade(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		if (base.Actor is Enemy enemy)
		{
			enemy.SetImmune(active: true);
		}
		base.Actor.Agent.Agent.updatePosition = true;
		this.m_Action = base.Actor.ActiveAction as CharacterActionEvade;
		this.m_IsMoving = false;
		base.Actor.ForceStop();
		base.Actor.SetMoveSpeed(0f);
		base.Actor.SetMoveXSpeed(0f);
		this.RemoveListeners();
		this.AddListeners();
		base.Actor.Content?.SetAnimationTrigger(this.m_Action.AnimationTrigger);
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Evade);
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
		if (this.m_Action != null)
		{
			Vector3Direction direction;
			direction = this.m_Action.Direction;
			float speed;
			speed = this.m_Action.Speed;
			float num;
			num = speed / 2f;
			float num2;
			num2 = global::UnityEngine.Random.Range(0f - num, num);
			switch (direction)
			{
			case Vector3Direction.Forward:
				base.Actor.SetMoveSpeed(speed);
				base.Actor.SetMoveXSpeed(num2);
				break;
			case Vector3Direction.Back:
				base.Actor.SetMoveSpeed(0f - speed);
				base.Actor.SetMoveXSpeed(num2);
				break;
			case Vector3Direction.Left:
				base.Actor.SetMoveXSpeed(0f - speed);
				base.Actor.SetMoveSpeed(num2);
				break;
			case Vector3Direction.Right:
				base.Actor.SetMoveXSpeed(speed);
				base.Actor.SetMoveSpeed(num2);
				break;
			}
			this.m_IsMoving = true;
		}
	}

	private void HandleActorOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= this.HandleActorOnAnimationComplete;
		this.m_IsMoving = false;
		base.Actor.ForceStop(smooth: false);
		base.Actor.Agent.Agent.updatePosition = true;
		if (base.Actor is Enemy enemy)
		{
			enemy.SetImmune(active: false);
		}
	}

	public override void InternalOnStateExit()
	{
		this.RemoveListeners();
		base.Actor.SetMoveSpeed(base.Actor.Movement.MoveSpeed);
		base.Actor.SetMoveXSpeed(0f);
		base.Actor.ForceStop(smooth: false);
		this.m_Action = null;
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

	protected override void OnDisposed()
	{
		this.m_Action = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
