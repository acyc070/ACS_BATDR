using UnityEngine;

public class CharacterStateIdle : CharacterState
{
	public CharacterStateIdle(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.ForceStop();
	}

	protected override void InternalUpdate()
	{
		if (base.Actor.Target != null)
		{
			if (base.Actor.Target.GetComponent<Player>() == null)
			{
				base.Actor.SetState(State.Character.Patrol);
			}
			else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
			{
				if (base.Actor.CurrentNode != null)
				{
					base.Actor.SetTarget(base.Actor.CurrentNode.transform);
				}
				else
				{
					base.Actor.SetTarget(null);
				}
				base.Actor.SetState(State.Character.Patrol);
			}
			else
			{
				base.Actor.SetState(State.Character.Follow);
			}
		}
		else if (base.Actor.CharacterVision != null)
		{
			float num;
			num = Vector3.Distance(base.Actor.transform.position, GameManager.Instance.Player.transform.position);
			if ((!(base.Actor.Target != GameManager.Instance.Player.transform) || !(num <= base.Actor.CharacterVision.CloseRange) || GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide || !this.SetTargetPlayer()) && (!(num < 3.5f) || !this.SetTargetPlayer()) && VisionUtility.CheckFOV(angle: (GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth) ? 20f : 180f, forward: base.Actor.transform.forward, from: base.Actor.VisionPosition, to: GameManager.Instance.Player.transform, radius: base.Actor.CharacterVision.AwareRange, layerMask: ~((1 << LayerMask.NameToLayer("AI")) & LayerMask.NameToLayer("Audio"))))
			{
				this.SetTargetPlayer();
			}
		}
	}

	private bool SetTargetPlayer()
	{
		if (GameManager.Instance.Player.CurrentState != State.Player.Ability)
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

	protected virtual void GetRotationInput()
	{
		base.Actor.Rotation?.UpdateInput();
	}

	protected virtual void GetMovementInput()
	{
		base.Actor.Movement?.UpdateInput();
	}

	protected virtual void GetRotation()
	{
		base.Actor.Rotation?.Update();
	}

	protected virtual void GetMovement()
	{
		base.Actor.Movement?.Update();
	}
}
