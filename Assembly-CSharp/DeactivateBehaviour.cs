using UnityEngine;
using UnityEngine.Playables;

public class DeactivateBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		this.Target.SetActive(value: false);
	}
}
