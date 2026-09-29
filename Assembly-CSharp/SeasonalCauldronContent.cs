using System;

public class SeasonalCauldronContent : ActionEventContent<SeasonalCauldronContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private SeasonalCauldron m_Cauldron;

	protected override void OnInitialize()
	{
		this.m_Cauldron = (SeasonalCauldron)base.Connectable;
		this.InitializeContent();
	}

	public void InitializeContent()
	{
		base.RemoveListeners();
		base.AddListeners();
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
	}

	protected override void OnDisposed()
	{
		this.m_Cauldron = null;
		base.OnDisposed();
	}
}
