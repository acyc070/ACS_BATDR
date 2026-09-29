using System;

public class CoffeeMachineContent : ActionEventContent<CoffeeMachineContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public LightFixture LightFixture;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 15)
		{
			if (GameManager.Instance.Player.Health < UpgradeCheck.GetHealth())
			{
				this.Enable();
			}
		}
		else if (!base.IsInactive && (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs < 15 || GameManager.Instance.Player.Health >= UpgradeCheck.GetHealth()))
		{
			this.Disable();
		}
	}

	protected override void InternalEnable()
	{
		this.EnableLighting();
	}

	private void EnableLighting()
	{
		if (base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(1f);
			}
		}
	}

	protected override void InternalDisable()
	{
		this.DisableLighting();
	}

	private void DisableLighting()
	{
		if (base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(0f);
			}
		}
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_SPENT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", 15);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveSlugs(15);
		GameManager.Instance.Player.SetHealth(unchecked((int)UpgradeCheck.GetHealth()));
		this.Disable();
		base.DeactivateComplete();
	}
}
