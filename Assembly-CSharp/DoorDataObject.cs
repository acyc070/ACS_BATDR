using System;
using UnityEngine;

[Serializable]
public class DoorDataObject : DataObject<DoorID, DoorDataObject>, IDataObject<DoorID>, IDataObject
{
	[SerializeField]
	private DoorID m_DoorID;

	[SerializeField]
	private DoorStatus m_Status;

	[SerializeField]
	private DoorType m_Type;

	public override DoorID ID => this.m_DoorID;

	public DoorStatus Status => this.m_Status;

	public DoorType Type => this.m_Type;

	public void SetStatus(DoorStatus status)
	{
		this.m_Status = status;
	}

	public void SetType(DoorType type)
	{
		this.m_Type = type;
	}

	protected override void Deserialize()
	{
		this.m_DoorID = base.m_ID;
	}
}
