using System;
using System.Collections.Generic;

public abstract class PlayerStateMachine : StateMachine<Player, State.Player>
{
	private Dictionary<State.PlayerAbility, PlayerAbilityState> m_ActiveAbilities = new Dictionary<State.PlayerAbility, PlayerAbilityState>();

	public PlayerWeaponState CurrentWeapon { get; private set; }

	public List<PlayerAbilityState> Abilities { get; private set; }

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

	public void SetWeapon(PlayerWeaponState weapon)
	{
		this.CurrentWeapon?.OnStateExit();
		this.CurrentWeapon = weapon;
		this.CurrentWeapon?.OnStateEnter();
	}

	protected void ClearWeapon()
	{
		if (this.CurrentWeapon != null)
		{
			this.CurrentWeapon.Dispose();
			this.CurrentWeapon = null;
		}
	}

	public void SetAbility(State.PlayerAbility ability)
	{
		string typeName;
		typeName = typeof(State.PlayerAbility).ToString().RemoveSymbols().Replace("State", "") + "State" + ability;
		if (this.m_ActiveAbilities.ContainsKey(ability))
		{
			return;
		}
		PlayerAbilityState playerAbilityState;
		playerAbilityState = (PlayerAbilityState)Activator.CreateInstance(Type.GetType(typeName), this, ability);
		this.m_ActiveAbilities.Add(ability, playerAbilityState);
		if (!GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(ability))
		{
			AbilityDataObject value;
			value = DataObject<State.PlayerAbility, AbilityDataObject>.Create(ability);
			GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.Add(ability, value);
		}
		if (this.Abilities == null)
		{
			this.Abilities = new List<PlayerAbilityState>();
		}
		if (!this.Abilities.Contains(playerAbilityState))
		{
			this.Abilities.Add(playerAbilityState);
		}
		for (int i = 0; i < this.Abilities.Count; i = unchecked(i + 1))
		{
			PlayerAbilityState playerAbilityState2;
			playerAbilityState2 = this.Abilities[i];
			if (playerAbilityState2.ID != playerAbilityState.ID)
			{
				playerAbilityState2.OnStateExit();
			}
		}
		playerAbilityState.OnStateEnter();
	}

	public bool HasTeleport()
	{
		bool result;
		result = false;
		if (this.Abilities != null)
		{
			for (int i = 0; i < this.Abilities.Count; i = unchecked(i + 1))
			{
				if (this.Abilities[i] is PlayerAbilityStateFlow)
				{
					result = true;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		this.ClearWeapon();
		if (this.m_ActiveAbilities != null)
		{
			this.m_ActiveAbilities.Clear();
			this.m_ActiveAbilities = null;
		}
		if (this.Abilities != null)
		{
			this.Abilities.Clear();
			this.Abilities = null;
		}
		base.OnDisposed();
	}
}
