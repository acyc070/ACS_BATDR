using System;
using UnityEngine;

[Serializable]
public class Vector3Data
{
	[SerializeField]
	private float m_X;

	[SerializeField]
	private float m_Y;

	[SerializeField]
	private float m_Z;

	public Vector3 Vector3 => new Vector3(this.m_X, this.m_Y, this.m_Z);

	public static Vector3Data Get(Vector3 vector)
	{
		return new Vector3Data
		{
			m_X = vector.x,
			m_Y = vector.y,
			m_Z = vector.z
		};
	}
}
