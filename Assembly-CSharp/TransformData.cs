using System;
using UnityEngine;

[Serializable]
public class TransformData
{
	[SerializeField]
	private Vector3 m_Position;

	[SerializeField]
	private Quaternion m_Rotation;

	[SerializeField]
	private Vector3 m_EulerAngles;

	[SerializeField]
	private Vector3 m_LocalScale;

	public Vector3 Position => this.m_Position;

	public Quaternion Rotation => this.m_Rotation;

	public Vector3 EulerAngles => this.m_EulerAngles;

	public Vector3 LocalScale => this.m_LocalScale;

	public static TransformData Get(Transform transform)
	{
		return new TransformData
		{
			m_Position = transform.position,
			m_Rotation = transform.rotation,
			m_EulerAngles = transform.eulerAngles,
			m_LocalScale = transform.localScale
		};
	}
}
