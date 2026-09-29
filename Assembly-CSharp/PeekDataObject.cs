using System;
using UnityEngine;

[Serializable]
public class PeekDataObject : DataObject<int, PeekDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_PeekState;

	[SerializeField]
	private int m_Index;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => this.m_DataID;

	public int PeekState => this.m_PeekState;

	public int Index => this.m_Index;

	public Quaternion RotationX => this.m_RotationX;

	public Quaternion RotationY => this.m_RotationY;

	public void SetPeekState(bool isPeeking)
	{
		this.m_PeekState = (isPeeking ? 1 : 0);
	}

	public void SetIndex(int index)
	{
		this.m_Index = index;
	}

	public void SetRotationX(Quaternion rotation)
	{
		this.m_RotationX = rotation;
	}

	public void SetRotationY(Quaternion rotation)
	{
		this.m_RotationY = rotation;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
