public abstract class CharacterStateMachine : StateMachine<Character, State.Character>
{
	protected sealed override void Update()
	{
		base.GetActions(base.Update, this.InternalUpdate);
	}

	protected virtual void InternalUpdate()
	{
	}

	protected sealed override void FixedUpdate()
	{
		base.GetActions(base.FixedUpdate, this.InternalFixedUpdate);
	}

	protected virtual void InternalFixedUpdate()
	{
	}

	protected sealed override void LateUpdate()
	{
		base.GetActions(base.LateUpdate, this.InternalLateUpdate);
	}

	protected virtual void InternalLateUpdate()
	{
	}
}
