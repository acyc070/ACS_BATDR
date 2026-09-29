using UnityEngine;

public class Sparkles : JMonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_ParticleSystem;

	public ParticleSystem ParticleSystem => this.m_ParticleSystem;

	public void Play()
	{
		this.m_ParticleSystem.Play();
	}

	public void Stop()
	{
		this.m_ParticleSystem.Stop();
	}
}
