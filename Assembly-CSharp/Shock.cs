using UnityEngine;

public class Shock : JMonoBehaviour
{
	private ParticleSystem[] m_Particles;

	public override void Awake()
	{
		this.m_Particles = base.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
	}

	public void Play()
	{
		if (this.m_Particles != null)
		{
			for (int i = 0; i < this.m_Particles.Length; i = unchecked(i + 1))
			{
				this.m_Particles[i]?.Play();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.m_Particles = null;
		base.OnDisposed();
	}
}
