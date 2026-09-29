using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13ParticlePassthrough : MonoBehaviour
{
	[Tooltip("Optional. Teleports to location of collision/trigger. Placing audio as children of this object allows for more 3D sound")]
	[SerializeField]
	private GameObject follower;

	[Tooltip("To receive these calls, PS must have Collision and Send Collision Messages enabled")]
	[SerializeField]
	private S13LocalAction onParticleCollision = new S13LocalAction();

	[Tooltip("To receive these calls, PS must have Triggers enabled (with a collider) and Enter set to callback")]
	[SerializeField]
	private S13LocalAction onParticleTriggerEnter = new S13LocalAction();

	private ParticleSystem ps;

	private void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
	}

	private void OnParticleTrigger()
	{
		if (!base.enabled || this.ps == null || !this.onParticleTriggerEnter.IsExecutable)
		{
			return;
		}
		List<ParticleSystem.Particle> list;
		list = new List<ParticleSystem.Particle>();
		int triggerParticles;
		triggerParticles = this.ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, list);
		for (int i = 0; i < triggerParticles; i = unchecked(i + 1))
		{
			if ((bool)this.follower)
			{
				this.follower.transform.position = list[i].position;
			}
			this.onParticleTriggerEnter.Execute();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (!base.enabled || !this.ps || !this.onParticleCollision.IsExecutable)
		{
			return;
		}
		List<ParticleCollisionEvent> list;
		list = new List<ParticleCollisionEvent>();
		int collisionEvents;
		collisionEvents = this.ps.GetCollisionEvents(other, list);
		for (int i = 0; i < collisionEvents; i = unchecked(i + 1))
		{
			if ((bool)this.follower)
			{
				this.follower.transform.position = list[i].intersection;
			}
			this.onParticleCollision.Execute();
		}
	}
}
