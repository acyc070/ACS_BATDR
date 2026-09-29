using System;
using UnityEngine;

public class AnimationEventHeadTracker : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationSingle m_InteractableAnimation;

	[SerializeField]
	private HeadTracker m_HeadTracker;

	public override void Start()
	{
		if (this.m_InteractableAnimation != null && this.m_HeadTracker != null)
		{
			this.m_InteractableAnimation.OnInteract -= this.HandleOnInteract;
			this.m_InteractableAnimation.OnInteract += this.HandleOnInteract;
			this.m_InteractableAnimation.OnInteractionComplete -= this.HandleOnInteractionComplete;
			this.m_InteractableAnimation.OnInteractionComplete += this.HandleOnInteractionComplete;
		}
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		if (this.m_HeadTracker != null)
		{
			this.m_HeadTracker.SetActive(active: false);
		}
	}

	private void HandleOnInteractionComplete(object sender, EventArgs e)
	{
		if (this.m_HeadTracker != null)
		{
			this.m_HeadTracker.SetActive(active: true);
		}
	}

	protected override void OnDisposed()
	{
		this.m_InteractableAnimation.OnInteract -= this.HandleOnInteract;
		this.m_InteractableAnimation.OnInteractionComplete -= this.HandleOnInteractionComplete;
		base.OnDisposed();
	}
}
