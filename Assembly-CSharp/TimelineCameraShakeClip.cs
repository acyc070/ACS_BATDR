using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCameraShakeClip : PlayableAsset, ITimelineClipAsset
{
	public float Duration;

	public float Strength;

	public int Vibrato;

	public float Randomness;

	public bool FadeOut;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCameraShakeBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineCameraShakeBehaviour>.Create(graph);
		TimelineCameraShakeBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.Duration = this.Duration;
		behaviour.Strength = this.Strength;
		behaviour.Vibrato = this.Vibrato;
		behaviour.Randomness = this.Randomness;
		behaviour.FadeOut = this.FadeOut;
		return scriptPlayable;
	}
}
