using UnityEngine;
using UnityEngine.Playables;

public class TimelineSubtitleBehaviour : PlayableBehaviour
{
	public string Subtitles;

	public int Index;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (!Application.isPlaying || this.m_IsPlayed)
		{
			return;
		}
		this.m_IsPlayed = true;
		if (GameManager.Instance.PlayerSettings.Subtitles)
		{
			string[] array;
			array = TextUtility.GetKey(this.Subtitles).Split('{');
			if (this.Index < array.Length)
			{
				GameManager.Instance.ShowSubtitles(array[this.Index], (float)playable.GetDuration(), isTrimmed: true);
			}
		}
	}
}
