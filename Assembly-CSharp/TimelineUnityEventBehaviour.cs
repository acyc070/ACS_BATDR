using UnityEngine;
using UnityEngine.Playables;

public class TimelineUnityEventBehaviour : PlayableBehaviour
{
	public TimelineUnityEvent UnityEvent;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (this.UnityEvent != null)
			{
				this.UnityEvent.Action();
			}
		}
	}
}
