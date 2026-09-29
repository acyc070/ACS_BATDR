using System;
using UnityEngine;

public class ActionEventLight : JMonoBehaviour
{
	[SerializeField]
	private LightBulbController m_LightBulbController;

	private ActionEventController m_ActionEventController;

	public override void Start()
	{
		if (!(this.m_LightBulbController == null))
		{
			this.m_ActionEventController = base.GetComponent<ActionEventController>();
			if (this.m_ActionEventController != null)
			{
				this.m_ActionEventController.OnActivated += this.HAndleActionEventControllerOnActivated;
				this.m_ActionEventController.OnDeactivated += this.HandleActionEventControllerOnDeactivated;
			}
		}
	}

	private void HandleActionEventControllerOnDeactivated(object sender, EventArgs e)
	{
		this.m_LightBulbController?.TurnOff();
	}

	private void HAndleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		this.m_LightBulbController?.TurnOn();
	}
}
