using UnityEngine;
using UnityEngine.Playables;

public class TimelineScreenBlockerBehaviour : PlayableBehaviour
{
	public float Duration;

	public bool Show = true;

	public bool IsWhite;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (this.Show)
			{
				GameManager.Instance.ShowScreenBlocker(this.Duration, 0f, null, "BLOCKER", isIndependent: false, this.IsWhite);
			}
			else
			{
				GameManager.Instance.HideScreenBlocker(this.Duration, 0f, null, "BLOCKER", isIndependent: false, this.IsWhite);
			}
		}
	}
}
