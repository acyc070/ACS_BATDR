using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneBarsClip : PlayableAsset, ITimelineClipAsset
{
	[Header("IsEnd")]
	public bool IsEnd;

	public bool ShowCrosshair = true;

	[Header("Player")]
	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneBarsBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineCutsceneBarsBehaviour>.Create(graph);
		TimelineCutsceneBarsBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.IsEnd = this.IsEnd;
		behaviour.ShowCrosshair = this.ShowCrosshair;
		behaviour.SlowPlayer = this.SlowPlayer;
		behaviour.LockPlayer = this.LockPlayer;
		behaviour.SkipPlayIn = this.SkipPlayIn;
		behaviour.DisableSkip = this.DisableSkip;
		return scriptPlayable;
	}
}
