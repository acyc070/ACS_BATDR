public class PlayerStateCutscene : PlayerState
{
	public PlayerStateCutscene(Player player, State.Player id)
		: base(player, id)
	{
	}

	protected override void InternalOnStateEnter()
	{
		base.Actor.CancelMovement();
		base.Actor.PlayerMovement.StopRun();
	}

	protected override void InternalUpdate()
	{
		base.Actor.UpdateAbilitiesInput();
	}

	protected override void InternalFixedUpdate()
	{
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
