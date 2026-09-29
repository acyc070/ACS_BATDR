using System;
using UnityEngine;

[Serializable]
public class ComboLockDataObject : DataObject<int, ComboLockDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_Code;

	[SerializeField]
	private int m_IsComplete;

	public override int ID => this.m_DataID;

	public int Code => this.m_Code;

	public int IsComplete => this.m_IsComplete;

	public void SetCode(int code)
	{
		this.m_Code = code;
	}

	public void SetComplete()
	{
		this.m_IsComplete = 1;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
