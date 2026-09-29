using System;
using UnityEngine;

public class CompanionNode : CharacterNode
{
	[SerializeField]
	private CompanionNodeType m_NodeType;

	[SerializeField]
	private Transform m_SitLocation;

	[SerializeField]
	private ActionEventController m_ActionEventController;

	[SerializeField]
	private string m_EventCompleteReaction = "Negative";

	public CompanionNodeType NodeType => this.m_NodeType;

	public Transform SitLocation => this.m_SitLocation;

	public ActionEventController ActionEventController => this.m_ActionEventController;

	public string EventCompleteReaction => this.m_EventCompleteReaction;

	public event EventHandler OnComplete;

	public void Enable()
	{
		if (this.m_ActionEventController != null)
		{
			this.m_ActionEventController.Content.Enable();
		}
	}

	public void Disable()
	{
		if (this.m_ActionEventController != null && this.m_ActionEventController.Content != null && !this.m_ActionEventController.Content.IsActivated)
		{
			this.m_ActionEventController.Content.Disable();
		}
	}

	protected override void InternalOnNodeReached()
	{
		if (this.m_NodeType == CompanionNodeType.Point && this.m_ActionEventController != null)
		{
			this.m_ActionEventController.OnActivate -= this.HandleActionEventControllerOnActivate;
			this.m_ActionEventController.OnActivate += this.HandleActionEventControllerOnActivate;
			this.Enable();
		}
	}

	private void HandleActionEventControllerOnActivate(object sender, EventArgs e)
	{
		this.m_ActionEventController.OnActivate -= this.HandleActionEventControllerOnActivate;
		this.OnComplete.Send(this);
	}
}
