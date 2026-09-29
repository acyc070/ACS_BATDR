using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineCutsceneCameraClip : PlayableAsset, ITimelineClipAsset
{
	public TimelineCutsceneCameraType CutsceneType;

	public ExposedReference<Transform> Target;

	public ExposedReference<Transform> StartLocation;

	public ExposedReference<Transform> EndLocation;

	public bool UnlockWeapons = true;

	public bool EndCrouching;

	public bool IsReal;

	public bool IsSnap;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<TimelineCutsceneCameraBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<TimelineCutsceneCameraBehaviour>.Create(graph);
		TimelineCutsceneCameraBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.SetOwner(go);
		behaviour.Target = this.Target.Resolve(graph.GetResolver());
		behaviour.StartLocation = this.StartLocation.Resolve(graph.GetResolver());
		behaviour.EndLocation = this.EndLocation.Resolve(graph.GetResolver());
		behaviour.CutsceneType = this.CutsceneType;
		behaviour.UnlockWeapons = this.UnlockWeapons;
		behaviour.EndCrouching = this.EndCrouching;
		behaviour.IsReal = this.IsReal;
		behaviour.IsSnap = this.IsSnap;
		return scriptPlayable;
	}
}
