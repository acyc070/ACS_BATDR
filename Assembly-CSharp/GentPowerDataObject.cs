using System;
using UnityEngine;

[Serializable]
public class GentPowerDataObject : DataObject<GentPowerID, GentPowerDataObject>, IDataObject<GentPowerID>, IDataObject
{
	[SerializeField]
	private GentPowerID m_GentPowerID;

	[SerializeField]
	private CutsceneStatus m_Status;

	public override GentPowerID ID => this.m_GentPowerID;

	public CutsceneStatus Status => this.m_Status;

	public void SetStatus(CutsceneStatus status)
	{
		this.m_Status = status;
	}

	protected override void Deserialize()
	{
		this.m_GentPowerID = base.m_ID;
	}
}
