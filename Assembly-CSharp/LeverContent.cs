using System;

public class LeverContent : ActionEventContent<LeverContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		Properties properties;
		properties = base.m_Properties[0];
		if (base.IsInactive)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (properties.ActionEvent.IsActive)
			{
				properties.ActionEvent.ForceDisable();
			}
		}
		else if (!properties.ActionEvent.IsActive)
		{
			this.Enable();
		}
	}

	protected override void OnActivate()
	{
		this.InternalActivation();
	}

	protected override void OnForceActivateComplete()
	{
		base.GetActiveProperties();
		int hashCode;
		hashCode = this.GetHashCode();
		unchecked
		{
			for (int i = 0; i < base.m_ActiveProperties.Count; i++)
			{
				Properties properties;
				properties = base.m_ActiveProperties[i];
				if (hashCode != properties.ActionEvent.GetHashCode())
				{
					InteractableAnimationSingle interactableAnimationSingle;
					interactableAnimationSingle = properties.ActionEvent as InteractableAnimationSingle;
					if (interactableAnimationSingle != null)
					{
						for (int j = 0; j < interactableAnimationSingle.Animators.Length; j++)
						{
							interactableAnimationSingle.Animators[j].SetTrigger("InteractForceComplete");
						}
						interactableAnimationSingle.Dispose();
					}
				}
				hashCode = properties.ActionEvent.GetHashCode();
			}
			this.Disable();
			base.IsActivated = true;
		}
	}

	private void InternalActivation()
	{
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			this.ActivationAnimation(base.m_ActiveProperties[i]);
		}
	}

	private void ActivationAnimation(Properties properties)
	{
		InteractableAnimationBase obj;
		obj = properties.ActionEvent as InteractableAnimationBase;
		obj.OnInteract -= this.HandleActivationOnInteract;
		obj.OnInteract += this.HandleActivationOnInteract;
		obj.OnInteractionComplete -= this.HandleActivationOnInteractionComplete;
		obj.OnInteractionComplete += this.HandleActivationOnInteractionComplete;
	}

	private void HandleActivationOnInteract(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteract -= this.HandleActivationOnInteract;
		GameManager.Instance.Player.SetCollision(active: false);
	}

	private void HandleActivationOnInteractionComplete(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteractionComplete -= this.HandleActivationOnInteractionComplete;
		GameManager.Instance.Player.SetCollision(active: true);
		base.ActivateComplete();
	}

	protected override void OnDeactivate()
	{
		this.InternalDeactivation();
	}

	protected override void OnForceDeactivateComplete()
	{
		base.GetActiveProperties();
		int hashCode;
		hashCode = this.GetHashCode();
		unchecked
		{
			for (int i = 0; i < base.m_ActiveProperties.Count; i++)
			{
				Properties properties;
				properties = base.m_ActiveProperties[i];
				if (hashCode != properties.ActionEvent.GetHashCode())
				{
					InteractableAnimationSingle interactableAnimationSingle;
					interactableAnimationSingle = properties.ActionEvent as InteractableAnimationSingle;
					if (interactableAnimationSingle != null)
					{
						for (int j = 0; j < interactableAnimationSingle.Animators.Length; j++)
						{
							interactableAnimationSingle.Animators[j].SetTrigger("Reset");
						}
					}
				}
				hashCode = properties.ActionEvent.GetHashCode();
			}
			base.IsActivated = false;
		}
	}

	private void InternalDeactivation()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			this.DeactivationAnimation(base.m_Properties[i]);
		}
	}

	private void DeactivationAnimation(Properties properties)
	{
		if (properties != null && properties.ActionEvent != null && properties.ActionEvent is InteractableAnimationBase interactableAnimationBase && interactableAnimationBase != null)
		{
			interactableAnimationBase.OnInteract -= this.HandleDeactivationOnInteract;
			interactableAnimationBase.OnInteract += this.HandleDeactivationOnInteract;
			interactableAnimationBase.OnInteractionComplete -= this.HandleDeactivationOnInteractionComplete;
			interactableAnimationBase.OnInteractionComplete += this.HandleDeactivationOnInteractionComplete;
		}
	}

	private void HandleDeactivationOnInteract(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteract -= this.HandleDeactivationOnInteract;
		GameManager.Instance.Player.SetCollision(active: false);
	}

	private void HandleDeactivationOnInteractionComplete(object sender, EventArgs e)
	{
		(sender as InteractableAnimationBase).OnInteractionComplete -= this.HandleDeactivationOnInteractionComplete;
		GameManager.Instance.Player.SetCollision(active: true);
		base.DeactivateComplete();
	}

	private void InternalRemoveListeners()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties != null && properties.ActionEvent != null && properties.ActionEvent is InteractableAnimationBase interactableAnimationBase && interactableAnimationBase != null)
			{
				interactableAnimationBase.OnInteract -= this.HandleActivationOnInteract;
				interactableAnimationBase.OnInteractionComplete -= this.HandleActivationOnInteractionComplete;
				interactableAnimationBase.OnInteract -= this.HandleDeactivationOnInteract;
				interactableAnimationBase.OnInteractionComplete -= this.HandleDeactivationOnInteractionComplete;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.InternalRemoveListeners();
		base.OnDisposed();
	}
}
