using UnityEngine;

public class PlayerAbilityStateFastTravel : PlayerAbilityState
{
	public PlayerAbilityStateFastTravel(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	public override void InternalAbilityInitializer()
	{
		base.m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_FastTravel");
		base.m_AbilityUseDelay = 0f;
		base.m_AbilityDistanceMax = 6f;
	}

	protected override bool InternalCheckUse()
	{
		bool canUse;
		canUse = base.CanUse;
		if (canUse)
		{
			base.m_HandMaterial.SetTexture("_Emission", base.m_Sigil);
		}
		return canUse;
	}

	protected override bool InternalUse()
	{
		return true;
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
	}

	protected override void ClearCheck()
	{
		if (base.CanUse)
		{
			base.CanUse = false;
			base.ResetAbilityEmission();
		}
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
