using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class DOTweenBehaviour : PlayableBehaviour
{
	public DOTweenAnimation DOTweenAnimation;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying)
		{
			this.DOTweenAnimation?.DOPlay();
		}
	}
}
