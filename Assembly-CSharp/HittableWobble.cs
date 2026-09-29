using DG.Tweening;
using UnityEngine;

public class HittableWobble : Hittable
{
	[SerializeField]
	private Transform m_Pivot;

	[SerializeField]
	private float m_Value = 10f;

	[SerializeField]
	private float m_Duration = 1f;

	protected override bool InternalHit(RaycastHit hit)
	{
		this.m_Pivot.DOKill();
		this.m_Pivot.localEulerAngles = Vector3.zero;
		this.m_Pivot.DOPunchRotation(Random.insideUnitSphere.normalized * this.m_Value, this.m_Duration).SetEase(Ease.InOutElastic);
		return true;
	}

	protected override void OnDisposed()
	{
		this.m_Pivot.DOKill();
		base.OnDisposed();
	}
}
