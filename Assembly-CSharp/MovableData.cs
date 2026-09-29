using System;
using UnityEngine;

[Serializable]
public class MovableData
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private int m_CurrentIndex;

	[SerializeField]
	private int m_MoveState;

	[SerializeField]
	private int m_Side;

	public bool IsActive => this.m_IsActive;

	public int CurrentIndex => this.m_CurrentIndex;

	public int MoveState => this.m_MoveState;

	public int Side => this.m_Side;

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
}
