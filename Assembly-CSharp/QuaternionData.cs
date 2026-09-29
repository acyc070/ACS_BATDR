using System;
using UnityEngine;

[Serializable]
public class QuaternionData
{
	[SerializeField]
	private float m_X;

	[SerializeField]
	private float m_Y;

	[SerializeField]
	private float m_Z;

	[SerializeField]
	private float m_W;

	public Quaternion Quaternion => new Quaternion(this.m_X, this.m_Y, this.m_Z, this.m_W);

	public static QuaternionData Get(Quaternion quaternion)
	{
		return new QuaternionData
		{
			m_X = quaternion.x,
			m_Y = quaternion.y,
			m_Z = quaternion.z,
			m_W = quaternion.w
		};
	}
}
