public class CharacterStateControllable : CharacterState
{
	public CharacterStateControllable(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.SetCharacterAction(null);
		base.Actor.Content.Animator.SetCombatState(0f);
		base.Actor.ClearAnimationTriggers();
		base.Actor.ForceStop();
		base.Actor.IsAlerted = false;
	}

	protected override void InternalUpdate()
	{
		this.GetMovementInput();
		this.GetAnimations();
	}

	protected override void InternalFixedUpdate()
	{
		this.GetMovement();
	}

	protected virtual void GetMovementInput()
	{
		base.Actor.Movement?.UpdateInput();
	}

	protected void GetMovement()
	{
		base.Actor.Movement?.Update();
	}

	protected void GetAnimations()
	{
		float num;
		num = ((base.Actor.MoveYInput != 0f) ? 1 : 0);
		if (num == 1f && base.Actor.RunInput)
		{
			num = 2f;
		}
		base.Actor.Content.Animator.SetMovementState(num);
		if (base.Actor.MoveYInput < 0f)
		{
			num = -1f;
		}
		base.Actor.Content.Animator.SetMovementSpeed(num);
		base.Actor.SetRun(base.Actor.RunInput);
	}
}
