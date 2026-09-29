using System;

public class CollectableContent : ActionEventContent<CollectableContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public InteractableAnimation InteractableAnimation;

		public bool DisposeOnAction;
	}

	protected override void OnInitialized()
	{
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.OnActivate += this.HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		bool flag;
		flag = false;
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (!flag)
			{
				flag = properties.DisposeOnAction;
			}
		}
		if (flag)
		{
			base.Connectable.Dispose();
		}
	}

	protected override void InternalEnable()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			base.m_Properties[i].InteractableAnimation?.SetActive(active: true);
		}
	}
}
