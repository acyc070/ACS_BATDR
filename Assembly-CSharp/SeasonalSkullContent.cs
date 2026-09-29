using System;

public class SeasonalSkullContent : ActionEventContent<SeasonalSkullContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private SeasonalSkull m_Skull;

	protected override void OnInitialize()
	{
		this.m_Skull = (SeasonalSkull)base.Connectable;
		this.InitializeContent();
	}

	public void InitializeContent()
	{
		base.RemoveListeners();
		base.AddListeners();
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.OnActivate += this.HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey(this.m_Skull.Data.SkullName));
		SeasonalSkullCheck.Collect();
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		}
		this.m_Skull = null;
		base.OnDisposed();
	}
}
