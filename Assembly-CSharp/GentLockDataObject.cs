using System;
using UnityEngine;

[Serializable]
public class GentLockDataObject : DataObject<GentLockID, GentLockDataObject>, IDataObject<GentLockID>, IDataObject
{
	[SerializeField]
	private GentLockID m_GentLockID;

	[SerializeField]
	private CutsceneStatus m_Status;

	[SerializeField]
	private double m_Timeline;

	public override GentLockID ID => this.m_GentLockID;

	public CutsceneStatus Status => this.m_Status;

	public double Timeline => this.m_Timeline;

	public void SetStatus(CutsceneStatus status)
	{
		this.m_Status = status;
	}

	public void SetTimeline(double timeline)
	{
		this.m_Timeline = timeline;
	}

	protected override void Deserialize()
	{
		this.m_GentLockID = base.m_ID;
	}
}
