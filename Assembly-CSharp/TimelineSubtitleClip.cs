using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineSubtitleClip : PlayableAsset, ITimelineClipAsset
{
	public string Subtitles;

	public int Index;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineSubtitleBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineSubtitleBehaviour>.Create(graph);
		TimelineSubtitleBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.Subtitles = this.Subtitles;
		behaviour.Index = this.Index;
		return scriptPlayable;
	}
}
