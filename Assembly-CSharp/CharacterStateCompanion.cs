using UnityEngine;

public class CharacterStateCompanion : CharacterState
{
	public CharacterStateCompanion(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
	}

	protected override void InternalUpdate()
	{
		if (!(base.Actor.ActiveAction != null) && !(base.Actor.Target == null))
		{
			if (Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position) < base.Actor.CharacterVision.CloseRange && !base.Actor.Agent.Agent.isOnOffMeshLink)
			{
				base.Actor.Agent.Agent.updatePosition = false;
				Quaternion b;
				b = Quaternion.LookRotation((base.Actor.Target.position - base.Actor.transform.position).normalized);
				base.Actor.transform.rotation = Quaternion.Slerp(base.Actor.transform.rotation, b, Time.deltaTime * 5f);
			}
			else
			{
				base.Actor.Agent.Agent.updatePosition = true;
				base.Actor.Agent.MoveTo(base.Actor.Target.position);
			}
			this.GetAnimationStatesTEMP();
		}
	}

	protected void GetAnimationStatesTEMP()
	{
		if (!(base.Actor == null) && !(base.Actor.Content == null) && !(base.Actor.Content.Animator == null))
		{
			float num;
			num = Vector3.Distance(base.Actor.Target.position, base.Actor.transform.position);
			float num2;
			num2 = (base.Actor.CharacterVision.AwareRange - base.Actor.CharacterVision.CloseRange) / 2f;
			float value;
			value = 0f;
			bool smooth;
			smooth = true;
			bool run;
			run = false;
			if (num > base.Actor.CharacterVision.CloseRange + num2 && !base.Actor.IsStopAndGo)
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.RunSpeed;
				value = 2f;
				run = true;
			}
			else if (num > base.Actor.CharacterVision.CloseRange)
			{
				base.Actor.Agent.Agent.speed = base.Actor.Movement.MoveSpeed;
				value = 1f;
				run = false;
			}
			else if (num < base.Actor.CharacterVision.CloseRange)
			{
				base.Actor.Agent.Agent.angularSpeed = 360f;
				base.Actor.Agent.Agent.velocity = Vector3.zero;
				value = 0f;
				run = false;
			}
			base.Actor.Content.Animator.SetMovementState(value, smooth);
			base.Actor.Content.Animator.SetMovementSpeed(value, smooth);
			base.Actor.SetRun(run);
		}
	}
}
