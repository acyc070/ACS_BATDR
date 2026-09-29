public class PlayerStateDefault : PlayerState
{
	public PlayerStateDefault(Player player, State.Player id)
		: base(player, id)
	{
	}

	protected override void InternalOnStateEnter()
	{
	}

	protected override void InternalUpdate()
	{
		base.Actor.UpdateLookInput();
		if (!GameManager.Instance.PlayerSettings.SmoothCamera)
		{
			base.Actor.UpdateRotations();
		}
		base.Actor.UpdateMovementInput();
		base.Actor.UpdateInteractionInput();
		base.Actor.UpdateAbilitiesInput();
		base.Actor.UpdateWeaponInput();
	}

	protected override void InternalFixedUpdate()
	{
		if (GameManager.Instance.PlayerSettings.SmoothCamera)
		{
			base.Actor.UpdateRotations();
		}
		base.Actor.UpdateMovement();
		base.Actor.UpdateAbilities();
		base.Actor.UpdateWeapon();
	}

	protected override void InternalLateUpdate()
	{
		base.Actor.UpdateAnimations();
		base.Actor.UpdateAbilitiesLateUpdate();
		base.Actor.UpdateWeaponLateUpdate();
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
