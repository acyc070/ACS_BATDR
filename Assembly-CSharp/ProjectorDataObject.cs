using System;
using UnityEngine;

[Serializable]
public class ProjectorDataObject : DataObject<int, ProjectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Index;

	public override int ID => this.m_DataID;

	public int Index => this.m_Index;

	public void SetIndex(int index)
	{
		this.m_Index = index;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
