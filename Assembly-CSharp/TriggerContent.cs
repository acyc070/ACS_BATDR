using System;

public class TriggerContent : ActionEventContent<TriggerContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public bool OnExit;
	}

	protected override void OnInitialized()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.OnExit)
			{
				properties.ActionEvent.OnExit += this.HandleActionEventOnExit;
				break;
			}
		}
	}

	protected override void OnActivate()
	{
	}

	private void HandleActionEventOnExit(object sender, EventArgs e)
	{
		base.RemoveListeners();
		this.ResetActionEvents();
		base.AddListeners();
	}
}
