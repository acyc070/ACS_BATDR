using System.Collections;
using UnityEngine;

public class ParticleSystemPause : MonoBehaviour
{
	public ParticleSystem ps;

	private void OnValidate()
	{
		if (!this.ps)
		{
			this.ps = base.GetComponent<ParticleSystem>();
		}
	}

	private void Awake()
	{
		if (!this.ps)
		{
			this.ps = base.GetComponent<ParticleSystem>();
		}
	}

	private IEnumerator Start()
	{
		int maxCount;
		maxCount = 100;
		SimpleCull sc;
		do
		{
			if (unchecked(maxCount--) <= 0)
			{
				yield break;
			}
			sc = Object.FindObjectOfType<SimpleCull>();
			yield return null;
		}
		while (sc == null);
		do
		{
			yield return null;
		}
		while (!sc.cullTransform);
		if (Vector3.Distance(base.transform.position, sc.cullTransform.position) > sc.cullingDistance)
		{
			this.OnBecameInvisible();
		}
	}

	private void OnBecameVisible()
	{
		if ((bool)this.ps && this.ps.isPaused)
		{
			this.ps.Play();
		}
	}

	private void OnBecameInvisible()
	{
		if ((bool)this.ps && this.ps.isPlaying)
		{
			this.ps.Pause();
		}
	}
}
