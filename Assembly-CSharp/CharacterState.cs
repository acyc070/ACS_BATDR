public class CharacterState : State<Character, State.Character>
{
	public CharacterState(Character character, State.Character id)
		: base(character)
	{
		base.ID = id;
	}

	public sealed override void OnStateEnter()
	{
		JDebug.Log("[" + base.Actor.name + "] - OnStateEnter :: " + base.ID, base.Actor, JDebug.JDebugType.AI);
		this.InternalOnStateEnter();
	}

	public virtual void InternalOnStateEnter()
	{
	}

	public sealed override void Update()
	{
		base.GetActions(this.InternalUpdate);
	}

	protected virtual void InternalUpdate()
	{
	}

	public sealed override void FixedUpdate()
	{
		base.GetActions(this.InternalFixedUpdate);
	}

	protected virtual void InternalFixedUpdate()
	{
	}

	public sealed override void LateUpdate()
	{
		base.GetActions(this.InternalLateUpdate);
	}

	protected virtual void InternalLateUpdate()
	{
	}

	public override void OnStateExit()
	{
		JDebug.Log("[" + base.Actor.name + "] - OnStateExit :: " + base.ID, base.Actor, JDebug.JDebugType.AI);
		this.InternalOnStateExit();
	}

	public virtual void InternalOnStateExit()
	{
	}

	protected bool CheckTarget()
	{
		if (base.Actor.Target != null)
		{
			return true;
		}
		return false;
	}
}
