public abstract class PlayerState : State<Player, State.Player>
{
	protected PlayerState(Player player, State.Player id)
		: base(player)
	{
		base.ID = id;
	}

	public sealed override void OnStateEnter()
	{
		this.DebugState("OnStateEnter");
		this.InternalOnStateEnter();
	}

	protected abstract void InternalOnStateEnter();

	public sealed override void Update()
	{
		base.GetActions(this.InternalUpdate);
	}

	protected abstract void InternalUpdate();

	public sealed override void FixedUpdate()
	{
		base.GetActions(this.InternalFixedUpdate);
	}

	protected abstract void InternalFixedUpdate();

	public sealed override void LateUpdate()
	{
		base.GetActions(this.InternalLateUpdate);
	}

	protected abstract void InternalLateUpdate();

	public sealed override void OnStateExit()
	{
		this.DebugState("OnStateExit");
		this.InternalOnStateExit();
	}

	protected abstract void InternalOnStateExit();

	protected void DebugState(string method)
	{
		JDebug.Log("[" + base.Actor.name + "] - " + method + " :: " + base.ID, base.Actor, JDebug.JDebugType.Player);
	}
}
