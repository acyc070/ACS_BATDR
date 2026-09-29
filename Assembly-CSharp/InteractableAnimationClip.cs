using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class InteractableAnimationClip : PlayableAsset, ITimelineClipAsset
{
	public ExposedReference<InteractableAnimation> InteractableAnimation;

	public bool OnEnter;

	public bool OnExit;

	public bool DoLock;

	public bool DoUnlock;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		ScriptPlayable<InteractableAnimationBehaviour> scriptPlayable;
		scriptPlayable = ScriptPlayable<InteractableAnimationBehaviour>.Create(graph);
		InteractableAnimationBehaviour behaviour;
		behaviour = scriptPlayable.GetBehaviour();
		behaviour.InteractableAnimation = this.InteractableAnimation.Resolve(graph.GetResolver());
		behaviour.OnEnter = this.OnEnter;
		behaviour.OnExit = this.OnExit;
		behaviour.DoLock = this.DoLock;
		behaviour.DoUnlock = this.DoUnlock;
		return scriptPlayable;
	}
}
