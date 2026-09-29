using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class LightFixtureClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<GameObject> Target;

	public bool On;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<LightFixtureBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<LightFixtureBehaviour>.Create(graph);
		LightFixtureBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.Target = this.Target.Resolve(graph.GetResolver());
		behaviour.On = this.On;
		return scriptPlayable;
	}
}
