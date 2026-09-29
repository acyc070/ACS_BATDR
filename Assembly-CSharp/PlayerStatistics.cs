using System;
using UnityEngine;

[Serializable]
public class PlayerStatistics
{
	[SerializeField]
	private int m_Health;

	[SerializeField]
	private int m_Life;

	[SerializeField]
	private int m_ILife;

	[SerializeField]
	private float m_Cooldown;

	[SerializeField]
	private int m_Food;

	[SerializeField]
	private int m_Slugs;

	[SerializeField]
	private int m_Spent;

	[SerializeField]
	private int m_Cards;

	[SerializeField]
	private int m_Toolkits;

	[SerializeField]
	private int m_Parts;

	[SerializeField]
	private int m_BatteryCasings;

	[SerializeField]
	private int m_Batteries;

	[SerializeField]
	private int m_DepositedBatteries;

	[SerializeField]
	private bool m_IsCrouched;

	[SerializeField]
	private int m_UpgradeLevelHealth;

	[SerializeField]
	private int m_UpgradeLevelStamina;

	[SerializeField]
	private int m_UpgradeLevelAbility;

	[SerializeField]
	private int m_Kills;

	[SerializeField]
	private int m_ShockKills;

	[SerializeField]
	private int m_BanishKill;

	[SerializeField]
	private CombatStatus m_CombatStatus;

	public int Health => this.m_Health;

	public int Life => this.m_Life;

	public int ILife => this.m_ILife;

	public float Cooldown => this.m_Cooldown;

	public int Food => this.m_Food;

	public int Slugs => this.m_Slugs;

	public int Spent => this.m_Spent;

	public int Cards => this.m_Cards;

	public int Toolkits => this.m_Toolkits;

	public int Parts => this.m_Parts;

	public int BatteryCasings => this.m_BatteryCasings;

	public int Batteries => this.m_Batteries;

	public int DepositedBatteries => this.m_DepositedBatteries;

	public bool IsCrouched => this.m_IsCrouched;

	public int UpgradeLevelHealth => this.m_UpgradeLevelHealth;

	public int UpgradeLevelStamina => this.m_UpgradeLevelStamina;

	public int UpgradeLevelAbility => this.m_UpgradeLevelAbility;

	public int Kills => this.m_Kills;

	public int ShockKills => this.m_ShockKills;

	public int BanishKill => this.m_BanishKill;

	public CombatStatus CombatStatus => this.m_CombatStatus;

	public void SetHealth(int amount)
	{
		this.m_Health = amount;
	}

	public void UpdateLife()
	{
		unchecked
		{
			this.m_Life++;
		}
	}

	public void UpdateILife()
	{
		unchecked
		{
			this.m_ILife++;
			if (this.m_ILife >= 25)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_DEMONS_DISMAY);
			}
		}
	}

	public void AddFood()
	{
		unchecked
		{
			this.m_Food++;
			if (this.m_Food >= 100)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_FANCY_FOODIE);
			}
		}
	}

	public void AddSlugs(int amount)
	{
		unchecked
		{
			this.m_Slugs += amount;
		}
	}

	public void RemoveSlugs(int amount)
	{
		unchecked
		{
			this.m_Slugs -= amount;
			if (this.m_Slugs < 0)
			{
				this.m_Slugs = 0;
			}
			this.m_Spent += amount;
			if (this.m_Spent >= 500)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BIG_SPENDER);
			}
		}
	}

	public void AddCard()
	{
		unchecked
		{
			this.m_Cards++;
		}
	}

	public void RemoveCard()
	{
		unchecked
		{
			this.m_Cards--;
			if (this.m_Cards < 0)
			{
				this.m_Cards = 0;
			}
		}
	}

	public void AddToolkit()
	{
		unchecked
		{
			this.m_Toolkits++;
		}
	}

	public void RemoveToolkit()
	{
		unchecked
		{
			this.m_Toolkits--;
			if (this.m_Toolkits < 0)
			{
				this.m_Toolkits = 0;
			}
		}
	}

	public void RemoveToolkits(int amount)
	{
		unchecked
		{
			this.m_Toolkits -= amount;
			if (this.m_Toolkits < 0)
			{
				this.m_Toolkits = 0;
			}
		}
	}

	public void AddPart()
	{
		unchecked
		{
			this.m_Parts++;
		}
	}

	public void RemoveParts(int amount)
	{
		unchecked
		{
			this.m_Parts -= amount;
			if (this.m_Parts < 0)
			{
				this.m_Parts = 0;
			}
		}
	}

	public void AddBattery()
	{
		unchecked
		{
			this.m_Batteries++;
		}
	}

	public void DepositBattery()
	{
		unchecked
		{
			this.m_DepositedBatteries += this.m_Batteries;
			this.m_Batteries = 0;
		}
	}

	public void RemoveBattery(int amount)
	{
		unchecked
		{
			this.m_Batteries -= amount;
			if (this.m_Batteries < 0)
			{
				this.m_Batteries = 0;
			}
		}
	}

	public void AddBatteryCasing()
	{
		unchecked
		{
			this.m_BatteryCasings++;
		}
	}

	public void RemoveBatteryCasing(int amount)
	{
		unchecked
		{
			this.m_BatteryCasings -= amount;
			if (this.m_BatteryCasings < 0)
			{
				this.m_BatteryCasings = 0;
			}
		}
	}

	public void SetCrouched(bool isCrouched)
	{
		this.m_IsCrouched = isCrouched;
	}

	public void UpgradeHealth()
	{
		unchecked
		{
			this.m_UpgradeLevelHealth++;
			if (this.m_UpgradeLevelHealth > 3)
			{
				this.m_UpgradeLevelHealth = 3;
			}
		}
	}

	public void UpgradeStamina()
	{
		unchecked
		{
			this.m_UpgradeLevelStamina++;
			if (this.m_UpgradeLevelStamina > 3)
			{
				this.m_UpgradeLevelStamina = 3;
			}
		}
	}

	public void UpgradeAbility()
	{
		unchecked
		{
			this.m_UpgradeLevelAbility++;
			if (this.m_UpgradeLevelAbility > 3)
			{
				this.m_UpgradeLevelAbility = 3;
			}
		}
	}

	public void AddKill()
	{
		unchecked
		{
			this.m_Kills++;
		}
	}

	public void AddShockKill()
	{
		unchecked
		{
			this.m_ShockKills++;
			if (this.m_ShockKills >= 50)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.HEAVY_HITTER);
			}
		}
	}

	public void AddBanish()
	{
		unchecked
		{
			this.m_BanishKill++;
			if (this.m_BanishKill >= 25)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BACK_TO_THE_PUDDLES);
			}
		}
	}

	public void SetCombatStatus(CombatStatus combatStatus)
	{
		this.m_CombatStatus = combatStatus;
	}
}
