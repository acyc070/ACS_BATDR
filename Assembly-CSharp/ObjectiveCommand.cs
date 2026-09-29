using System;

[Serializable]
public class ObjectiveCommand : Command<ObjectiveCommandType, Objective>
{
	protected override void InternalExecute()
	{
		base.m_CommandObject.OnComplete += this.HandleCommandObjectOnComplete;
		if (base.m_CommandType != ObjectiveCommandType.None)
		{
			if (base.m_CommandType == ObjectiveCommandType.Initialize)
			{
				base.m_CommandObject.Initialize();
			}
			else if (base.m_CommandType == ObjectiveCommandType.Enable)
			{
				base.m_CommandObject.Enable();
			}
			else if (base.m_CommandType == ObjectiveCommandType.Disable)
			{
				base.m_CommandObject.Disable();
			}
			else if (base.m_CommandType == ObjectiveCommandType.Inactive)
			{
				base.m_CommandObject.Inactive();
			}
			else if (base.m_CommandType == ObjectiveCommandType.ForceComplete)
			{
				base.m_CommandObject.ForceComplete();
			}
			else
			{
				this.AbortExecute();
			}
		}
	}

	protected override void InternalForceComplete()
	{
		if (!(base.m_CommandObject == null))
		{
			base.m_CommandObject.OnComplete -= this.HandleCommandObjectOnComplete;
			base.m_CommandObject.ForceComplete();
		}
	}

	private void HandleCommandObjectOnComplete(object sender, EventArgs e)
	{
		base.m_CommandObject.OnComplete -= this.HandleCommandObjectOnComplete;
		base.SendOnComplete();
	}

	private void AbortExecute()
	{
		JDebug.Log("[Abort Execute] :: " + base.m_CommandObject.name + " :: " + base.m_CommandType, base.m_CommandObject, JDebug.JDebugType.Objectives);
		base.m_CommandObject.OnComplete -= this.HandleCommandObjectOnComplete;
		base.SendOnComplete();
	}
}
