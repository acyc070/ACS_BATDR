using System;
using UnityEngine;

[Serializable]
public class LadderDataObject : DataObject<int, LadderDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_ClimbState;

	[SerializeField]
	private int m_ClimbDirection;

	[SerializeField]
	private int m_LadderSide;

	[SerializeField]
	private int m_LadderIndex;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override int ID => this.m_DataID;

	public int ClimbState => this.m_ClimbState;

	public int ClimbDirection => this.m_ClimbDirection;

	public int LadderSide => this.m_LadderSide;

	public int LadderIndex => this.m_LadderIndex;

	public Quaternion RotationX => this.m_RotationX;

	public Quaternion RotationY => this.m_RotationY;

	public void SetClimbState(bool isClimbing)
	{
		this.m_ClimbState = (isClimbing ? 1 : 0);
	}

	public void SetClimbDirection(int direction)
	{
		this.m_ClimbDirection = direction;
	}

	public void SetLadderSide(int side)
	{
		this.m_LadderSide = side;
	}

	public void SetLadderIndex(int index)
	{
		this.m_LadderIndex = index;
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
