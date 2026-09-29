using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DeactivateClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<GameObject> Target;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<DeactivateBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<DeactivateBehaviour>.Create(graph);
		scriptPlayable.GetBehaviour().Target = this.Target.Resolve(graph.GetResolver());
		return scriptPlayable;
	}
}
