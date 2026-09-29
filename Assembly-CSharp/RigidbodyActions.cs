using UnityEngine;

public class RigidbodyActions : JMonoBehaviour
{
	[SerializeField]
	private Rigidbody m_Rigidbody;

	[SerializeField]
	private Transform m_ForceDirection;

	[SerializeField]
	private float m_ForceAmount;

	public void AddForce()
	{
		this.m_Rigidbody.AddForce(this.m_ForceDirection.forward * this.m_ForceAmount, ForceMode.Impulse);
	}
}
