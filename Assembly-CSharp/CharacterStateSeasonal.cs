using System;
using UnityEngine;

public class CharacterStateSeasonal : CharacterState
{
	public CharacterStateSeasonal(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
		base.Actor.Agent.Agent.updatePosition = true;
		base.Actor.Agent.Agent.updateRotation = true;
	}

	public override void InternalOnStateExit()
	{
		if (base.Actor != null)
		{
			base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
		}
	}

	protected override void InternalUpdate()
	{
		if (base.Actor.Target == null || this.CheckPatrol() || base.Actor.Target == null)
		{
			this.ClearMovement();
			return;
		}
		base.Actor.Agent.MoveTo(base.Actor.Target.position);
		this.GetAnimationStates();
	}

	private bool CheckPatrol()
	{
		if (base.Actor.CurrentNode != null && Vector3.Distance(base.Actor.transform.position, base.Actor.CurrentNode.transform.position) <= base.Actor.Agent.Agent.stoppingDistance)
		{
			CharacterInteractionNode characterInteractionNode;
			characterInteractionNode = base.Actor.CurrentNode as CharacterInteractionNode;
			if (characterInteractionNode != null)
			{
				if (characterInteractionNode.NodeType == CharacterNodeType.Location || base.Actor is Enemy { EnemyType: EnemyType.Searcher })
				{
					base.Actor.SetTarget(null);
					base.Actor.ForceStop();
					base.Actor.Content.Animator.SetCombatState(0f);
					base.Actor.SendOnNodeReached();
				}
				else
				{
					if (characterInteractionNode.NodeType == CharacterNodeType.Interaction)
					{
						this.Interaction("Interact", "Interact");
						return true;
					}
					if (characterInteractionNode.NodeType == CharacterNodeType.InteractionLoop)
					{
						this.Interaction("SpecialAnimation01", "SpecialAnimationEnter", "SpecialAnimation02");
						return true;
					}
				}
			}
		}
		return false;
	}

	private void Interaction(string clipName, string triggerName, string loopClipName = "")
	{
		CharacterInteractionNode node;
		node = base.Actor.CurrentNode as CharacterInteractionNode;
		if (!(node != null))
		{
			return;
		}
		base.Actor.SetTarget(null);
		base.Actor.ForceStop();
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.SlideTo(base.Actor.CurrentNode.transform, 0.25f, () =>
		{
			node.Interactable.Trigger();
			node.InteractionClip.name = clipName;
			if (loopClipName != "" && node.InteractionLoopClip != null)
			{
				node.InteractionLoopClip.name = loopClipName;
			}
			if (node.InteractionLoopClip != null)
			{
				base.Actor.Content.UpdateClipOverrides(node.InteractionClip, node.InteractionLoopClip);
			}
			else
			{
				base.Actor.Content.UpdateClipOverrides(node.InteractionClip);
			}
			base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
			base.Actor.OnAnimationComplete += this.HandleAnimationComplete;
			base.Actor.Content.SetAnimationTrigger(triggerName);
		});
	}

	private void HandleAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
		CharacterInteractionNode characterInteractionNode;
		characterInteractionNode = base.Actor.CurrentNode as CharacterInteractionNode;
		if (characterInteractionNode != null && base.Actor.CurrentNode != null && characterInteractionNode.EndLocation != null)
		{
			base.Actor.transform.position = characterInteractionNode.EndLocation.position;
			base.Actor.transform.eulerAngles = characterInteractionNode.EndLocation.eulerAngles;
		}
		if (base.Actor is SeasonalEnemy seasonalEnemy)
		{
			seasonalEnemy.Leave();
		}
	}

	protected void GetAnimationStates()
	{
		if (!(base.Actor == null) && !(base.Actor.Content == null) && !(base.Actor.Content.Animator == null))
		{
			float num;
			num = Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position);
			float movementSpeed;
			movementSpeed = 0f;
			float movementState;
			movementState = 0f;
			bool moveSpeedSmooth;
			moveSpeedSmooth = true;
			bool movementSpeedRun;
			movementSpeedRun = false;
			if (num <= base.Actor.Agent.Agent.stoppingDistance)
			{
				base.Actor.Agent.Agent.angularSpeed = 360f;
				base.Actor.Agent.Agent.velocity = Vector3.zero;
				movementSpeed = 0f;
				movementState = 0f;
				movementSpeedRun = true;
			}
			else if (num < base.Actor.Agent.Agent.stoppingDistance + 7f)
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.MoveSpeed;
				movementSpeed = 1f;
				movementState = 1f;
				movementSpeedRun = false;
			}
			else if (num > base.Actor.Agent.Agent.stoppingDistance)
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.RunSpeed;
				movementSpeed = 2f;
				movementState = 1f;
				movementSpeedRun = false;
			}
			this.SetMovement(movementState, movementSpeed, moveSpeedSmooth, movementSpeedRun);
		}
	}

	private void SetMovement(float movementState, float movementSpeed, bool moveSpeedSmooth, bool movementSpeedRun)
	{
		base.Actor.Content.Animator.SetMovementState(movementState, moveSpeedSmooth);
		base.Actor.Content.Animator.SetMovementSpeed(movementSpeed, moveSpeedSmooth);
		base.Actor.SetRun(movementSpeedRun);
	}

	private void ClearMovement()
	{
		this.SetMovement(0f, 0f, moveSpeedSmooth: false, movementSpeedRun: false);
	}

	protected override void OnDisposed()
	{
		if (base.Actor != null)
		{
			base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
		}
		base.OnDisposed();
	}
}
