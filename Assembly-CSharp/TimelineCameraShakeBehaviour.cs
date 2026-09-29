using UnityEngine;
using UnityEngine.Playables;

public class TimelineCameraShakeBehaviour : PlayableBehaviour
{
	public float Duration;

	public float Strength = 90f;

	public int Vibrato = 10;

	public float Randomness = 90f;

	public bool FadeOut = true;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (!(this.Duration <= 0f))
			{
				CameraEffects.ShakeRotation(this.Duration, this.Strength, this.Vibrato, this.Randomness, this.FadeOut);
			}
		}
	}
}
