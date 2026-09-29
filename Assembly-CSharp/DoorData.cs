using System;
using UnityEngine;

[Serializable]
public class DoorData
{
	[SerializeField]
	private DoorStatus m_Status;

	[SerializeField]
	private DoorType m_Type;

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
}
