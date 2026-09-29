using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineScreenBlockerClip : PlayableAsset, ITimelineClipAsset
{
	public float Duration;

	public bool Show = true;

	public bool IsWhite;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineScreenBlockerBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineScreenBlockerBehaviour>.Create(graph);
		TimelineScreenBlockerBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.Duration = this.Duration;
		behaviour.Show = this.Show;
		behaviour.IsWhite = this.IsWhite;
		return scriptPlayable;
	}
}
