using System;
using UnityEngine;

[Serializable]
public class MovableDataObject : DataObject<MovableID, MovableDataObject>, IDataObject<MovableID>, IDataObject
{
	[SerializeField]
	private MovableID m_MovableID;

	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private int m_CurrentIndex;

	[SerializeField]
	private int m_MoveState;

	[SerializeField]
	private int m_Side;

	[SerializeField]
	private Quaternion m_RotationX;

	[SerializeField]
	private Quaternion m_RotationY;

	public override MovableID ID => this.m_MovableID;

	public bool IsActive => this.m_IsActive;

	public int CurrentIndex => this.m_CurrentIndex;

	public int MoveState => this.m_MoveState;

	public int Side => this.m_Side;

	public Quaternion RotationX => this.m_RotationX;

	public Quaternion RotationY => this.m_RotationY;

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public void SetCurrentIndex(int currentIndex)
	{
		this.m_CurrentIndex = currentIndex;
	}

	public void SetMoveState(bool canMove)
	{
		this.m_MoveState = (canMove ? 1 : 0);
	}

	public void SetSide(int side)
	{
		this.m_Side = side;
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
		this.m_MovableID = base.m_ID;
	}
}
