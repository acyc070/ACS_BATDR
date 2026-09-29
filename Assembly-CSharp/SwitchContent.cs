using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SwitchContent : ActionEventContent<SwitchContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	[SerializeField]
	private UnityEvent OnActivated;

	[SerializeField]
	private UnityEvent OnDeactivated;

	protected override void OnInitialized()
	{
		base.EventActivated = this.OnActivated;
		base.EventDeactivated = this.OnDeactivated;
	}

	protected override void OnActivate()
	{
		this.InternalActivation();
	}

	protected override void OnForceActivateComplete()
	{
		this.InternalActivation(isAnimation: false);
	}

	private void InternalActivation(bool isAnimation = true)
	{
		base.GetActiveProperties();
		if (isAnimation)
		{
			base.m_Sequencer?.New();
		}
		int animatorHash;
		animatorHash = this.GetHashCode();
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_ActiveProperties[i];
			if (properties.SwitchType == SwitchType.DOTween)
			{
				this.ActivationDOTween(properties, ref animatorHash, isAnimation);
			}
			else if (properties.SwitchType == SwitchType.Animation)
			{
				this.ActivationAnimation(properties);
			}
		}
	}

	private void ActivationDOTween(Properties properties, ref int animatorHash, bool isAnimation = true)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			if (isAnimation)
			{
				base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, 0.5f, Ease.OutBack);
			}
			else
			{
				base.ForceAnimationComplete(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles);
			}
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	private void ActivationAnimation(Properties properties)
	{
		if (properties.AnimatorController != null)
		{
			properties.AnimatorController.SetTrigger(properties.AnimationTriggerDeactivate);
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationOnComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandleAnimationOnComplete;
		GameManager.Instance.Player.EnterInteraction(properties.PlayerInteractionTriggerEnter);
		GameManager.Instance.Player.SlideToLocation(properties.AnimationLocation);
	}

	private void HandleAnimationOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationOnComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.SetCollision(active: true);
		base.ActivateComplete();
	}

	protected override void OnDeactivate()
	{
		this.InternalDeactivation();
	}

	protected override void OnForceDeactivateComplete()
	{
		this.InternalDeactivation(isAnimation: false);
	}

	private void InternalDeactivation(bool isAnimation = true)
	{
		if (isAnimation)
		{
			base.m_Sequencer?.New();
		}
		int hashCode;
		hashCode = this.GetHashCode();
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.SwitchType == SwitchType.DOTween)
			{
				if (hashCode != properties.Animator.GetHashCode())
				{
					if (isAnimation)
					{
						base.DoAnimation(properties, properties.m_OriginPosition, properties.m_OriginRotation, 0.4f, Ease.InBack);
					}
					else
					{
						base.ForceAnimationComplete(properties, properties.m_OriginPosition, properties.m_OriginRotation);
					}
				}
				hashCode = properties.Animator.GetHashCode();
			}
			else if (properties.SwitchType == SwitchType.Animation)
			{
				if (isAnimation)
				{
					this.DeactivationAnimation(properties);
				}
				else if (properties.AnimatorController != null)
				{
					properties.AnimatorController.SetTrigger("Reset");
				}
			}
		}
		if (!isAnimation)
		{
			base.DeactivateComplete();
		}
	}

	private void DeactivationAnimation(Properties properties)
	{
		if (properties.AnimatorController != null)
		{
			properties.AnimatorController.SetTrigger(properties.AnimationTriggerActivate);
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.EnterInteraction(properties.PlayerInteractionTriggerExit);
		GameManager.Instance.Player.SlideToLocation(properties.AnimationLocation);
	}

	private void HandleAnimationDeactivateOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationDeactivateOnComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.Player.SetCollision(active: true);
		base.DeactivateComplete();
	}

	private void InternalRemoveListeners()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationOnComplete;
			GameManager.Instance.Player.OnAnimationComplete -= this.HandleAnimationDeactivateOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.InternalRemoveListeners();
		base.OnDisposed();
	}
}
