using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneControllerVibrationClip : PlayableAsset, ITimelineClipAsset
{
	public bool IsEnd;

	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	public float Vibration_Duration;

	public float Vibration_Strength;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneControllerVibrationBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineCutsceneControllerVibrationBehaviour>.Create(graph);
		TimelineCutsceneControllerVibrationBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.IsEnd = this.IsEnd;
		behaviour.SlowPlayer = this.SlowPlayer;
		behaviour.LockPlayer = this.LockPlayer;
		behaviour.SkipPlayIn = this.SkipPlayIn;
		behaviour.DisableSkip = this.DisableSkip;
		behaviour.m_Duration = this.Vibration_Duration;
		behaviour.m_Strength = this.Vibration_Strength;
		return scriptPlayable;
	}
}
