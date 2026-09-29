using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineActionClip : PlayableAsset, ITimelineClipAsset
{
	[SerializeField]
	private List<TimelineActions> m_Actions = new List<TimelineActions>();

	public List<TimelineActions> Actions => this.m_Actions;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineActionBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineActionBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().Actions = this.Actions;
		return scriptPlayable;
	}
}
