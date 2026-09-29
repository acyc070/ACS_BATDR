using UnityEngine;
using UnityEngine.Playables;

public class TimelineAnimatorLinkBehaviour : PlayableBehaviour
{
	public Animator Animator;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		_ = Application.isPlaying;
	}
}
