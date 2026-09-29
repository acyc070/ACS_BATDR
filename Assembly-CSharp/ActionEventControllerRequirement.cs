using System;
using UnityEngine;

public class ActionEventControllerRequirement : JMonoBehaviour
{
	[SerializeField]
	private ActionEventController m_Controller;

	[SerializeField]
	private Requirements m_Requirements;

	public event EventHandler OnAction;

	public override void Start()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.m_Controller.OnDeactivate -= this.HandleControllerOnDeactivate;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			this.m_Controller.OnDeactivate += this.HandleControllerOnDeactivate;
		}
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.m_Controller.OnDeactivate -= this.HandleControllerOnDeactivate;
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ActionEventControllerRequirement :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	private void HandleControllerOnDeactivate(object sender, EventArgs e)
	{
		JDebug.Log("ActionEventControllerRequirement :: HandleControllerOnActivated", this, JDebug.JDebugType.Objectives);
		this.OnAction.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnAction = null;
		base.OnDisposed();
	}
}
