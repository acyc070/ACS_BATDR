using System;
using UnityEngine;

[Serializable]
public class RigidbodyData
{
	[SerializeField]
	private Vector3Data m_Velocity;

	[SerializeField]
	private Vector3Data m_AngularVelocity;

	[SerializeField]
	private bool m_IsKinematic;

	[SerializeField]
	private bool m_UseGravity;

	public Vector3 Velocity => this.m_Velocity.Vector3;

	public Vector3 AngularVelocity => this.m_AngularVelocity.Vector3;

	public bool IsKinematic => this.m_IsKinematic;

	public bool UseGravity => this.m_UseGravity;

	public static RigidbodyData Get(Rigidbody rigidbody)
	{
		return new RigidbodyData
		{
			m_Velocity = Vector3Data.Get(rigidbody.velocity),
			m_AngularVelocity = Vector3Data.Get(rigidbody.angularVelocity),
			m_IsKinematic = rigidbody.isKinematic,
			m_UseGravity = rigidbody.useGravity
		};
	}
}
