using System;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateFlee : CharacterState
{
	private bool m_IsStopAndGo;

	public CharacterStateFlee(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.SetPreviousState(State.Character.Flee);
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
		if (base.Actor.Target != null && Vector3.Distance(base.Actor.transform.position, base.Actor.Target.position) > base.Actor.Agent.Agent.stoppingDistance + 1f)
		{
			base.Actor.Content.Animator.SetTrigger("Flee");
		}
		if (base.Actor is Enemy enemy && (enemy.EnemyType != EnemyType.KingWidow || enemy.EnemyType != EnemyType.ShipAhoy) && GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Flee);
		base.Actor.Agent.Agent.updatePosition = true;
		base.Actor.Agent.Agent.updateRotation = true;
		if (base.Actor.IsStopAndGo)
		{
			this.m_IsStopAndGo = false;
			base.Actor.OnAnimationEvent -= this.Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= this.Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= this.Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= this.Actor_OnAnimationComplete;
			base.Actor.OnAnimationEvent += this.Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 += this.Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter += this.Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete += this.Actor_OnAnimationComplete;
		}
		base.Actor.IsAlerted = false;
	}

	public override void InternalOnStateExit()
	{
		if (base.Actor.IsStopAndGo)
		{
			base.Actor.OnAnimationEvent -= this.Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= this.Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= this.Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= this.Actor_OnAnimationComplete;
			base.Actor.Agent.Agent.angularSpeed = 360f;
			this.m_IsStopAndGo = false;
		}
	}

	private void Actor_OnAnimationEvent(object sender, EventArgs e)
	{
		base.Actor.Agent.Agent.angularSpeed = 360f;
	}

	private void Actor_OnAnimationEvent2(object sender, EventArgs e)
	{
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	private void Actor_OnAnimationEnter(object sender, EventArgs e)
	{
		this.m_IsStopAndGo = true;
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	private void Actor_OnAnimationComplete(object sender, EventArgs e)
	{
		this.m_IsStopAndGo = false;
		base.Actor.Agent.Agent.angularSpeed = 0f;
	}

	protected override void InternalUpdate()
	{
		if (!this.CheckPlayerTarget() && !(base.Actor.Target == null))
		{
			base.Actor.Agent.MoveTo(base.Actor.Target.position);
			this.GetAnimationStatesTEMP();
		}
	}

	private bool CheckPlayerTarget()
	{
		bool result;
		result = false;
		if (GameManager.Instance.Player != null && base.Actor.Target == GameManager.Instance.Player.transform && base.Actor.CurrentNode != null)
		{
			base.Actor.SetTarget(base.Actor.CurrentNode.transform);
			result = true;
		}
		return result;
	}

	protected void GetAnimationStatesTEMP()
	{
		if (base.Actor == null || base.Actor.Content == null || base.Actor.Content.Animator == null)
		{
			return;
		}
		float num;
		num = Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position);
		float value;
		value = 0f;
		float value2;
		value2 = 0f;
		bool smooth;
		smooth = true;
		bool run;
		run = false;
		if (num > base.Actor.Agent.Agent.stoppingDistance)
		{
			if (base.Actor.IsStopAndGo)
			{
				if (this.m_IsStopAndGo)
				{
					base.Actor.Agent.Agent.speed = 5f;
					base.Actor.Agent.Agent.velocity = base.Actor.transform.forward * 5f;
					value = 1f;
					value2 = 1f;
					run = false;
				}
				else
				{
					base.Actor.Agent.Agent.velocity = Vector3.zero;
					value = 1f;
					value2 = 1f;
					run = false;
				}
			}
			else
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.RunSpeed;
				value = 2f;
				value2 = 2f;
				run = true;
			}
		}
		else if (num <= base.Actor.Agent.Agent.stoppingDistance)
		{
			base.Actor.Agent.Agent.angularSpeed = 360f;
			base.Actor.Agent.Agent.velocity = Vector3.zero;
			value = 0f;
			value2 = 0f;
			run = false;
		}
		base.Actor.Content.Animator.SetMovementState(value2, smooth);
		base.Actor.Content.Animator.SetMovementSpeed(value, smooth);
		base.Actor.SetRun(run);
	}
}
