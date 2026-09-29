public class PlayerStateCutscenePeek : PlayerState
{
	public PlayerStateCutscenePeek(Player player, State.Player id)
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
		base.Actor.UpdateLookInput();
		if (!GameManager.Instance.PlayerSettings.SmoothCamera)
		{
			base.Actor.UpdateAnimationRotations();
		}
		base.Actor.UpdateAbilitiesInput();
	}

	protected override void InternalFixedUpdate()
	{
		if (GameManager.Instance.PlayerSettings.SmoothCamera)
		{
			base.Actor.UpdateAnimationRotations();
		}
		base.Actor.UpdateAbilities();
	}

	protected override void InternalLateUpdate()
	{
		base.Actor.UpdateAnimations();
		base.Actor.UpdateAbilitiesLateUpdate();
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
