using System;
using UnityEngine;

[Serializable]
public class CutsceneDataObject : DataObject<CutsceneID, CutsceneDataObject>, IDataObject<CutsceneID>, IDataObject
{
	[SerializeField]
	private CutsceneID m_CutsceneID;

	[SerializeField]
	private CutsceneStatus m_Status;

	[SerializeField]
	private double m_Timeline;

	public override CutsceneID ID => this.m_CutsceneID;

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
		this.m_CutsceneID = base.m_ID;
	}
}
