using UnityEngine.Playables;
using UnityEngine.Video;

public class TimelineCutsceneVideoBehaviour : PlayableBehaviour
{
	public VideoPlayer VideoPlayer;

	public override void PrepareFrame(Playable playable, FrameData info)
	{
		if (this.VideoPlayer != null)
		{
			if (!this.VideoPlayer.isPlaying)
			{
				this.VideoPlayer.Play();
			}
			if (this.VideoPlayer.isPlaying)
			{
				double time;
				time = playable.GetTime();
				this.VideoPlayer.time = time;
			}
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if (this.VideoPlayer.isPlaying)
		{
			this.VideoPlayer.Pause();
		}
	}
}
