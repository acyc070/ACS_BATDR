public abstract class PlayerWeaponState : WeaponState<Player>
{
	protected Weapon m_Weapon;

	public Weapon Weapon => this.m_Weapon;

	public PlayerWeaponState(Player player, WeaponData data)
		: base(player, data)
	{
	}

	protected sealed override void InternalOnStateEnter()
	{
		this.OnInitialize();
	}

	public abstract void OnInitialize();

	protected override void InternalUpdateInput()
	{
		if (base.Actor.CurrentState != State.Player.Default || base.Actor.IsWeaponLocked || !base.IsActive)
		{
			return;
		}
		if (PlayerInput.AttackCharge())
		{
			if (!this.IsCharged)
			{
				this.Charge();
			}
			else if (!this.IsOvercharged)
			{
				this.Overcharge();
			}
		}
		else if (PlayerInput.AttackHold() && this.Use())
		{
			base.Actor.Attack();
			base.SetActive(active: false);
		}
	}

	public bool Charge()
	{
		if (!this.CheckCharge())
		{
			return false;
		}
		return this.InternalCharge();
	}

	protected abstract bool InternalCharge();

	protected bool CheckCharge()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return this.InternalCheckCharge();
	}

	protected abstract bool InternalCheckCharge();

	public bool Overcharge()
	{
		if (!this.CheckOvercharge())
		{
			return false;
		}
		return this.InternalOvercharge();
	}

	protected abstract bool InternalOvercharge();

	protected bool CheckOvercharge()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return this.InternalCheckOvercharge();
	}

	protected abstract bool InternalCheckOvercharge();

	public sealed override bool Use()
	{
		if (!this.CheckUse())
		{
			return this.Fail();
		}
		return this.InternalUse();
	}

	protected abstract bool InternalUse();

	protected sealed override bool CheckUse()
	{
		if (!base.IsActive)
		{
			return false;
		}
		return this.InternalCheckUse();
	}

	protected abstract bool InternalCheckUse();

	protected bool Fail()
	{
		this.Reset();
		return false;
	}

	public void OnAttack()
	{
		if (this.CheckOnAttack())
		{
			this.InternalOnAttack();
		}
	}

	protected virtual bool CheckOnAttack()
	{
		return true;
	}

	protected abstract void InternalOnAttack();

	public void OnAttackComplete()
	{
		this.InternalOnAttackComplete();
	}

	protected virtual void InternalOnAttackComplete()
	{
	}

	public sealed override void Reset()
	{
		this.InternalReset();
	}

	protected abstract void InternalReset();

	protected override void InternalOnStateExit()
	{
		base.Dispose();
	}

	protected override void OnDisposed()
	{
		if (this.m_Weapon != null)
		{
			this.m_Weapon.Dispose();
			this.m_Weapon = null;
		}
		base.OnDisposed();
	}
}
