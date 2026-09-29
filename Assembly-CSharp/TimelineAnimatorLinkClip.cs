using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineAnimatorLinkClip : PlayableAsset, ITimelineClipAsset
{
	public Animator Animator;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineAnimatorLinkBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineAnimatorLinkBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().Animator = this.Animator;
		return scriptPlayable;
	}
}
