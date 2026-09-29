using System;

public class PresentContent : ActionEventContent<PresentContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private Present m_Present;

	protected override void OnInitialize()
	{
		this.m_Present = (Present)base.Connectable;
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
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey(this.m_Present.Data.PresentName));
		PresentCheck.Collect();
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		}
		this.m_Present = null;
		base.OnDisposed();
	}
}
