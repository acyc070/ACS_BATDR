using UnityEngine;

public class HittableCharacter : Hittable
{
	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private Renderer[] m_Renderers;

	[SerializeField]
	private GameObject m_Particles;

	public override void Awake()
	{
		this.SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		if (this.m_Particles != null)
		{
			this.m_Particles.SetActive(active);
		}
		if (this.m_Collider != null)
		{
			this.m_Collider.enabled = active;
		}
	}

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Smoke").transform.transform.position = hit.point;
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, this.m_Renderers);
		return true;
	}
}
