using UnityEngine;

public class SectionParticleController : JMonoBehaviour
{
	private ParticleSystem[] m_Particles;

	public override void Start()
	{
		this.m_Particles = base.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
	}

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || this.m_Particles == null || this.m_Particles.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float farClipPlane;
		farClipPlane = GameManager.Instance.GameCamera.Camera.farClipPlane;
		Transform transform;
		transform = GameManager.Instance.GameCamera.transform;
		ParticleSystem[] particles;
		particles = this.m_Particles;
		foreach (ParticleSystem particleSystem in particles)
		{
			float minDistance;
			minDistance = Vector3.Distance(transform.position, particleSystem.transform.position);
			float num;
			num = 100f;
			float maxDistance;
			maxDistance = farClipPlane + num;
			if (Vector3.Angle(to: (particleSystem.transform.position - transform.position).normalized, from: transform.forward) < 80f)
			{
				this.CheckDistance(minDistance, maxDistance, particleSystem);
			}
			else
			{
				this.CheckDistance(minDistance, num, particleSystem);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance, ParticleSystem particle)
	{
		if (minDistance > maxDistance)
		{
			this.DisableLight(particle);
		}
		else
		{
			this.EnableLight(particle);
		}
	}

	private void EnableLight(ParticleSystem particle)
	{
		if (particle.isPaused)
		{
			particle.Play();
		}
	}

	private void DisableLight(ParticleSystem particle)
	{
		if (particle.isPlaying && particle.main.playOnAwake)
		{
			particle.Pause();
		}
	}

	protected override void OnDisposed()
	{
		this.m_Particles = null;
		base.OnDisposed();
	}
}
