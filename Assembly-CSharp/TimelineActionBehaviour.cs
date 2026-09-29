using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActionBehaviour : PlayableBehaviour
{
	public List<TimelineActions> Actions;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			for (int i = 0; i < this.Actions.Count; i = unchecked(i + 1))
			{
				this.Actions[i].Action();
			}
		}
	}
}
