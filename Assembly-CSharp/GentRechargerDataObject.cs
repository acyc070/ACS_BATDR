using System;
using UnityEngine;

[Serializable]
public class GentRechargerDataObject : DataObject<int, GentRechargerDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Batteries;

	public override int ID => this.m_DataID;

	public int Batteries => this.m_Batteries;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}

	public void SetBatteries(int batteries)
	{
		this.m_Batteries = batteries;
	}

	public void Clear()
	{
		this.m_Batteries = 0;
	}
}
