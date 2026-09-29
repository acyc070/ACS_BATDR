using UnityEngine;
using UnityEngine.Playables;

public class InteractableAnimationBehaviour : PlayableBehaviour
{
	public InteractableAnimation InteractableAnimation;

	public bool OnEnter = true;

	public bool OnExit;

	public bool DoLock;

	public bool DoUnlock;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying)
		{
			if (this.OnEnter)
			{
				this.InteractableAnimation.SetLocked(isLocked: true);
				this.InteractableAnimation.SetActive(active: false);
				this.InteractableAnimation.Enter();
			}
			else if (this.OnExit)
			{
				this.InteractableAnimation.Exit();
				this.InteractableAnimation.SetLocked(isLocked: false);
			}
			else if (this.DoLock)
			{
				this.InteractableAnimation.SetLocked(isLocked: true);
				this.InteractableAnimation.SetActive(active: false);
			}
			else if (this.DoUnlock)
			{
				this.InteractableAnimation.SetLocked(isLocked: false);
			}
		}
	}
}
