using UnityEngine;

public class HittableObject : Hittable
{
	[SerializeField]
	private GameObject m_HitEffectPrefab;

	[SerializeField]
	private bool m_DisposeOnHit;

	private GameObject m_HitEffect;

	private Rigidbody m_Rigidbody;

	public override void Awake()
	{
		base.Awake();
		if (this.m_Rigidbody == null)
		{
			this.m_Rigidbody = base.GetComponent<Rigidbody>();
		}
	}

	protected override bool InternalHit(RaycastHit hit)
	{
		if ((bool)this.m_HitEffectPrefab)
		{
			this.m_HitEffect = Object.Instantiate(this.m_HitEffectPrefab);
			this.m_HitEffect.transform.position = hit.point;
			Quaternion rotation;
			rotation = Quaternion.LookRotation(hit.point - base.transform.position);
			this.m_HitEffect.transform.rotation = rotation;
			this.m_HitEffect.transform.SetParent(base.transform);
		}
		if (this.m_DisposeOnHit)
		{
			base.Dispose();
		}
		return true;
	}

	protected override void OnDisposed()
	{
		this.m_HitEffect = null;
		this.m_Rigidbody = null;
		base.OnDisposed();
	}
}
