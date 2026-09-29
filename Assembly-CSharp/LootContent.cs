using System;
using DG.Tweening;
using UnityEngine;

public class LootContent : ActionEventContent<LootContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public GameObject Blocker;

		public float TweenDuration = 0.4f;

		public Ease TweenEase = Ease.OutSine;
	}

	protected override void OnActivate()
	{
		this.InternalActivation();
	}

	protected override void OnForceActivateComplete()
	{
		base.RemoveListeners();
		base.GetActiveProperties();
		this.InternalActivation(isAnimation: false);
		base.IsActivated = true;
	}

	private void InternalActivation(bool isAnimation = true)
	{
		if (isAnimation && base.m_Sequencer != null)
		{
			base.m_Sequencer.New();
		}
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_ActiveProperties[i];
			if (isAnimation)
			{
				this.CheckAnimation(properties);
			}
			else
			{
				this.CheckAnimationComplete(properties);
			}
		}
		this.Disable();
	}

	private void CheckAnimation(Properties properties)
	{
		if (properties.SwitchType == SwitchType.Animation)
		{
			properties.AnimatorController.SetTrigger("Interact");
		}
		else if (properties.SwitchType == SwitchType.DOTween)
		{
			base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.TweenDuration, properties.TweenEase);
		}
	}

	private void CheckAnimationComplete(Properties properties)
	{
		if (properties.SwitchType == SwitchType.Animation)
		{
			properties.AnimatorController.SetTrigger("InteractForceComplete");
		}
		else if (properties.SwitchType == SwitchType.DOTween)
		{
			base.ForceAnimationComplete(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles);
		}
		if (properties.ActionEvent != null)
		{
			properties.ActionEvent.Dispose();
		}
	}

	protected override void OnActivateComplete()
	{
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_ActiveProperties[i];
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
		}
	}
}
