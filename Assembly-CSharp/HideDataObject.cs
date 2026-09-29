using System;
using UnityEngine;

[Serializable]
public class HideDataObject : DataObject<int, HideDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_HideState;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => this.m_DataID;

	public int HideState => this.m_HideState;

	public Quaternion RotationX => this.m_RotationX;

	public Quaternion RotationY => this.m_RotationY;

	public void SetHideState(bool isHiding)
	{
		this.m_HideState = (isHiding ? 1 : 0);
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
