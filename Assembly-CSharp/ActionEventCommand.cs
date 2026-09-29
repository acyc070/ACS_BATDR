using System;

[Serializable]
public class ActionEventCommand
{
	public int ID;

	public int EventHandlerID;

	public string EventHandler;

	public ActionEventController ActionEventController;

	public ActionEventCommandType ActionEventCommandType;

	public bool isReady;

	public bool isExecuted;

	public event EventHandler OnEvent;

	public void Execute()
	{
		if (this.ActionEventController == null)
		{
			return;
		}
		if (this.ActionEventCommandType == ActionEventCommandType.Action)
		{
			this.ActionEventController.Content.Action();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.Activate)
		{
			this.ActionEventController.Content.Activate();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.ForceActivate)
		{
			this.ActionEventController.Content.ForceActivate();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.Deactivate)
		{
			this.ActionEventController.Content.Deactivate();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.ForceDeactivate)
		{
			this.ActionEventController.Content.ForceDeactivate();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.Disable)
		{
			this.ActionEventController.Content.Disable();
		}
		else if (this.ActionEventCommandType == ActionEventCommandType.Enable)
		{
			this.ActionEventController.Content.Enable();
		}
		else
		{
			if (this.ActionEventCommandType != ActionEventCommandType.ForceInactive)
			{
				return;
			}
			this.ActionEventController.Content.ForceInactive();
		}
		this.isExecuted = true;
	}

	private void HandleOnEvent(object sender, EventArgs e)
	{
		this.OnEvent.Send(this);
	}

	public void AddListeners()
	{
		if (this.EventHandler == "OnActivate")
		{
			this.ActionEventController.OnActivate += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnActivated")
		{
			this.ActionEventController.OnActivated += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnDeactivate")
		{
			this.ActionEventController.OnDeactivate += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnDeactivated")
		{
			this.ActionEventController.OnDeactivated += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnInactive")
		{
			this.ActionEventController.OnInactive += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnDisabled")
		{
			this.ActionEventController.OnDisabled += this.HandleOnEvent;
		}
		else if (this.EventHandler == "OnEnabled")
		{
			this.ActionEventController.OnEnabled += this.HandleOnEvent;
		}
	}

	public void RemoveListeners()
	{
		this.ActionEventController.OnActivate -= this.HandleOnEvent;
		this.ActionEventController.OnActivated -= this.HandleOnEvent;
		this.ActionEventController.OnDeactivate -= this.HandleOnEvent;
		this.ActionEventController.OnDeactivated -= this.HandleOnEvent;
		this.ActionEventController.OnInactive -= this.HandleOnEvent;
		this.ActionEventController.OnDisabled -= this.HandleOnEvent;
		this.ActionEventController.OnEnabled -= this.HandleOnEvent;
	}
}
