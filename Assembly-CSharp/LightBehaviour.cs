using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class LightBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public float FadeValue;

	public float FadeDuration;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !(this.Target == null) && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			Light component;
			component = this.Target.GetComponent<Light>();
			if (component != null)
			{
				component.DOKill();
				component.DOIntensity(this.FadeValue, this.FadeDuration).SetEase(Ease.Linear);
			}
		}
	}
}
