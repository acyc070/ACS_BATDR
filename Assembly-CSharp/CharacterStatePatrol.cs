using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStatePatrol(Character character, State.Character state) : CharacterState(character, state)
{
	private bool m_IsStopAndGo;

	private float m_InactiveTimer;

	private float m_InactiveTimerLimit = 5f;

	private Vector3 m_CurrentPosition;

	private Vector3 m_PreviousPosition;

	public override void InternalOnStateEnter()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
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
		base.Actor.OnDamageTaken -= this.HandleCharacterOnDamageTaken;
		base.Actor.OnDamageTaken += this.HandleCharacterOnDamageTaken;
	}

	private void HandleCharacterOnDamageTaken(object sender, EventArgs e)
	{
		base.Actor.OnDamageTaken -= this.HandleCharacterOnDamageTaken;
		this.SetTargetPlayer(checkAbility: false);
	}

	public override void InternalOnStateExit()
	{
		if (base.Actor != null)
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
			base.Actor.OnDamageTaken -= this.HandleCharacterOnDamageTaken;
			base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
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
		if (!this.CheckPlayerTarget() && !(base.Actor.ActiveAction != null) && !this.EvadeInactive() && !this.EvadeEnemy() && !this.CheckForPlayer() && !(base.Actor.Target == null) && !this.CheckPatrol() && !(base.Actor.Target == null))
		{
			this.m_PreviousPosition = base.Actor.transform.position;
			base.Actor.Agent.MoveTo(base.Actor.Target.position);
			this.m_CurrentPosition = base.Actor.transform.position;
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

	private bool EvadeInactive()
	{
		if (Vector3.Distance(this.m_PreviousPosition, this.m_CurrentPosition) < 0.005f)
		{
			if (this.m_InactiveTimer >= this.m_InactiveTimerLimit)
			{
				Collider[] array;
				array = Physics.OverlapSphere(base.Actor.transform.position + Vector3.up * (base.Actor.Agent.Agent.height / 2f), base.Actor.Agent.Agent.radius * 2f, -1, QueryTriggerInteraction.Ignore);
				Collider[] array2;
				array2 = array;
				foreach (Collider collider in array2)
				{
					if (array != null && !(collider.transform == base.Actor.transform) && base.Actor.ActionGroupEvade != null && base.Actor.ActionGroupEvade.GetAction(base.Actor, collider.transform) != null)
					{
						this.m_InactiveTimer = 0f;
						return true;
					}
				}
			}
			else
			{
				this.m_InactiveTimer += Time.deltaTime;
			}
		}
		else
		{
			this.m_InactiveTimer = 0f;
		}
		return false;
	}

	private bool EvadeEnemy()
	{
		Collider[] array;
		array = Physics.OverlapSphere(base.Actor.transform.position + Vector3.up * (base.Actor.Agent.Agent.height / 2f), base.Actor.Agent.Agent.radius * 1.6f, 1 << LayerMask.NameToLayer("AI"), QueryTriggerInteraction.Ignore);
		if (array.Length > 1)
		{
			Collider[] array2;
			array2 = array;
			foreach (Collider collider in array2)
			{
				if (array != null && !(collider.transform == base.Actor.transform) && base.Actor.ActionGroupEvade != null && base.Actor.ActionGroupEvade.GetAction(base.Actor, collider.transform) != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckForPlayer()
	{
		if (GameManager.Instance.Player == null || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			return false;
		}
		if (base.Actor.CharacterVision != null)
		{
			float num;
			num = GameManager.Instance.Player.transform.position.y - base.Actor.transform.position.y;
			if (num > base.Actor.CharacterVision.VerticalDistance || num < 0f - base.Actor.CharacterVision.VerticalDistance)
			{
				return false;
			}
			float num2;
			num2 = Vector3.Distance(base.Actor.transform.position, GameManager.Instance.Player.transform.position);
			if (base.Actor.Target != GameManager.Instance.Player.transform && num2 <= base.Actor.CharacterVision.CloseRange && GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide && this.SetTargetPlayer())
			{
				return true;
			}
			if (num2 < base.Actor.CharacterVision.TooCloseRange && this.SetTargetPlayer())
			{
				return true;
			}
			if (VisionUtility.CheckFOV(angle: (GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth) ? base.Actor.CharacterVision.StealthAngle : base.Actor.CharacterVision.Angle, forward: base.Actor.transform.forward, from: base.Actor.VisionPosition, to: GameManager.Instance.Player.transform, radius: base.Actor.CharacterVision.AwareRange, layerMask: ~(1 << LayerMask.NameToLayer("AI"))))
			{
				NavMesh.SamplePosition(GameManager.Instance.Player.transform.position, out var hit, 25f, -1);
				base.Actor.Agent.GetPath(hit.position, out var navMeshPath);
				if (navMeshPath.status != NavMeshPathStatus.PathComplete)
				{
					return false;
				}
				if (this.SetTargetPlayer())
				{
					return true;
				}
			}
		}
		return false;
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

	private bool SetTargetPlayer(bool checkAbility = true)
	{
		bool flag;
		flag = true;
		if (checkAbility)
		{
			flag = GameManager.Instance.Player.CurrentState != State.Player.Ability;
		}
		if (flag)
		{
			base.Actor.SetTarget(GameManager.Instance.Player.transform);
			GameManager.Instance.BreakStealth();
			if (!base.Actor.IsAlerted)
			{
				base.Actor.SetState(State.Character.Alert);
			}
			else
			{
				base.Actor.SetState(State.Character.Follow);
			}
			return true;
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
		base.Actor.SlideTo(base.Actor.CurrentNode.transform, 0.75f, () =>
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
		base.Actor.SendOnNodeReached();
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
				}
				else
				{
					base.Actor.Agent.Agent.velocity = Vector3.zero;
				}
			}
			else if (base.Actor is Enemy enemy)
			{
				base.Actor.Agent.Agent.speed = enemy.PatrolSpeed;
			}
			else
			{
				base.Actor.Agent.Agent.speed = 2f;
			}
			value = 0f;
			value2 = 1f;
			run = false;
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

	protected override void OnDisposed()
	{
		if (base.Actor != null)
		{
			base.Actor.OnAnimationEvent -= this.Actor_OnAnimationEvent;
			base.Actor.OnAnimationEvent2 -= this.Actor_OnAnimationEvent2;
			base.Actor.OnAnimationEnter -= this.Actor_OnAnimationEnter;
			base.Actor.OnAnimationComplete -= this.Actor_OnAnimationComplete;
			base.Actor.OnDamageTaken -= this.HandleCharacterOnDamageTaken;
			base.Actor.OnAnimationComplete -= this.HandleAnimationComplete;
		}
		base.OnDisposed();
	}
}
