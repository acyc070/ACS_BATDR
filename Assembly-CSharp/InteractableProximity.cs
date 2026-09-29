using UnityEngine;

public class InteractableProximity : Interactable
{
	[Header("Proximity Options")]
	[SerializeField]
	protected float m_Proximity;

	protected override bool InternalEnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return this.CheckDistance(origin, hit);
	}

	protected override bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return this.CheckDistance(origin, hit);
	}

	protected override bool InternalExitCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return true;
	}

	private bool CheckDistance(Vector3 origin, RaycastHit hit)
	{
		Vector3 a;
		a = origin;
		a.y = hit.point.y;
		return Vector3.Distance(a, hit.point) < this.m_Proximity;
	}
}
