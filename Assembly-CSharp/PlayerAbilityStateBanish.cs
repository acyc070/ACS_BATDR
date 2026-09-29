using System;
using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public class PlayerAbilityStateBanish : PlayerAbilityState
{
	private LayerMask m_TargetCheckLayer;

	private LayerMask m_TargetColliderCheckLayer;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_TargetPositionOffset;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_ToTargetRotation;

	private Sequence m_Sequence;

	public Enemy Target { get; private set; }

	protected override bool UseInput()
	{
		return PlayerInput.InteractOnPressed();
	}

	public PlayerAbilityStateBanish(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	public override void InternalAbilityInitializer()
	{
		this.m_TargetCheckLayer = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Audio")) | (1 << LayerMask.NameToLayer("EventTrigger")));
		this.m_TargetColliderCheckLayer = ~(1 << LayerMask.NameToLayer("Player"));
		base.m_AbilityPostProcessEffect = GameManager.Instance.GameCamera.BasePostProcess.profile.GetSetting<Takedown>();
		base.m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
		base.m_AbilityUseDelay = 1.5f;
		base.m_AbilityUseDuration = 1.5f;
		base.m_AbilityDistanceMax = 5f;
	}

	protected override void InternalNotReady()
	{
		this.Target = null;
	}

	protected override bool InternalCheckUse()
	{
		bool flag;
		flag = this.Target != null;
		if (flag)
		{
			base.m_HandMaterial.SetTexture("_Emission", base.m_Sigil);
		}
		return flag;
	}

	protected override bool InternalUse()
	{
		this.Target.AudreyBanishClip.name = "Interact";
		base.Actor.UpdatePlayerContentClipOverrides(this.Target.AudreyBanishClip);
		base.Actor.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
		base.Actor.OnAnimationComplete += this.HandleEnterOnAnimationComplete;
		base.Actor.EnterInteraction("Interact");
		base.Actor.SetState(State.Player.Ability);
		this.Target.Takedown();
		this.ResetSequence();
		float num;
		num = 0f;
		this.m_Sequence.Insert(num, base.Actor.transform.DOMove(this.m_ToTargetPosition, 0.25f).SetEase(Ease.InSine));
		this.m_Sequence.Insert(num, base.Actor.transform.DORotate(this.m_ToTargetRotation, 0.25f).SetEase(Ease.InSine));
		GameManager.Instance.GameCamera.BanishParticles.Play();
		if (base.m_AbilityPostProcessEffect != null)
		{
			this.m_Sequence.Insert(num, DOTween.To(() => base.m_AbilityPostProcessEffect._Power.value, (float x) =>
			{
				base.m_AbilityPostProcessEffect._Power.value = x;
			}, 1f, 0.5f).SetEase(Ease.InSine));
			num += 0.5f;
			this.m_Sequence.Insert(num, DOTween.To(() => base.m_AbilityPostProcessEffect._Power.value, (float x) =>
			{
				base.m_AbilityPostProcessEffect._Power.value = x;
			}, 0.925f, 0.05f).SetLoops(17, LoopType.Yoyo).SetEase(Ease.Linear));
			this.m_Sequence.Insert(num + 0.75f, DOTween.To(() => base.m_AbilityPostProcessEffect._Power.value, (float x) =>
			{
				base.m_AbilityPostProcessEffect._Power.value = x;
			}, 0f, 1.5f).SetEase(Ease.InSine));
		}
		int healAmount;
		healAmount = this.GetHealAmount();
		this.m_Sequence.InsertCallback(2.25f, () =>
		{
			GameManager.Instance.Player.Heal(healAmount);
		});
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBanish();
		return true;
	}

	private int GetHealAmount()
	{
		unchecked
		{
			int num;
			num = (int)UpgradeCheck.GetHealth();
			int result;
			result = num / 4;
			switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
			{
			case DifficultyLevel.Normal:
				result = num / 3;
				break;
			case DifficultyLevel.Easy:
				result = num / 2;
				break;
			}
			return result;
		}
	}

	private void HandleEnterOnAnimationComplete(object sender, EventArgs e)
	{
		base.Actor.OnAnimationComplete -= this.HandleEnterOnAnimationComplete;
		GameManager.Instance.Player.ExitAnimation();
		GameManager.Instance.GameCamera.BanishParticles.Stop();
		base.UseOnComplete();
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
		{
			return;
		}
		if (base.Actor.IsCrouched && (base.Actor.CombatStatus == CombatStatus.Stealth || base.Actor.CombatStatus == CombatStatus.Hide) && Physics.SphereCast(base.Actor.HeadContainer.position, 0.1f, base.Actor.HeadContainer.forward, out this.m_TargetHit, base.m_AbilityDistanceMax, this.m_TargetCheckLayer))
		{
			Enemy component;
			component = this.m_TargetHit.transform.GetComponent<Enemy>();
			if (component != null && component.CanBanish)
			{
				if (Mathf.Abs(Vector3.Angle(component.transform.forward, base.Actor.transform.position - component.transform.position)) > 120f)
				{
					if (this.Target != null && this.Target != component)
					{
						this.ClearCheck();
					}
					this.CheckTarget(component);
				}
				else
				{
					this.ClearCheck();
				}
			}
			else
			{
				this.ClearCheck();
			}
		}
		else
		{
			this.ClearCheck();
		}
	}

	protected override void ClearCheck()
	{
		if (!base.CanUse)
		{
			return;
		}
		base.CanUse = false;
		if (this.Target != null)
		{
			SkinnedMeshRenderer[] componentsInChildren;
			componentsInChildren = this.Target.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				ShaderEffects.SetFloat("_PostHitGlow", 0.3f, skinnedMeshRenderer);
				ShaderEffects.SetFloat("_Gradient", 0.1f, skinnedMeshRenderer);
				ShaderEffects.SetColor("_GradientColor", ShaderConstants.GRADIENT_DEFAULT, skinnedMeshRenderer);
			}
		}
		GameManager.Instance.HideInteraction();
		this.Target = null;
		this.m_TargetHit = default;
		this.m_TargetColliderHit = default;
		this.m_TargetPosition = Vector3.zero;
		this.m_TargetPositionOffset = Vector3.zero;
		this.m_ToTargetPosition = base.Actor.transform.position;
		this.m_ToTargetRotation = base.Actor.transform.eulerAngles;
	}

	private void CheckTarget(Enemy _target)
	{
		float num;
		num = 2.5f;
		this.Target = _target;
		this.m_TargetPosition = this.Target.transform.position;
		this.m_TargetPositionOffset = this.m_TargetPosition - this.Target.transform.forward * num;
		float height;
		height = base.Actor.CharacterController.height;
		float radius;
		radius = base.Actor.CharacterController.radius;
		if (Physics.SphereCast(this.m_TargetPositionOffset, radius + 0.01f, Vector3.up, out this.m_TargetColliderHit, height - radius, this.m_TargetColliderCheckLayer, QueryTriggerInteraction.Ignore) || Physics.SphereCast(this.m_TargetPosition + Vector3.up * radius, radius + 0.01f, -this.Target.transform.forward, out this.m_TargetColliderHit, num, this.m_TargetColliderCheckLayer, QueryTriggerInteraction.Ignore))
		{
			this.ClearCheck();
			return;
		}
		this.m_ToTargetPosition = this.m_TargetPositionOffset;
		this.m_ToTargetRotation = this.Target.transform.eulerAngles;
		if (base.CanUse)
		{
			return;
		}
		base.CanUse = true;
		if (this.Target != null)
		{
			SkinnedMeshRenderer[] componentsInChildren;
			componentsInChildren = this.Target.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				ShaderEffects.SetFloat("_PostHitGlow", 2f, skinnedMeshRenderer);
				ShaderEffects.SetFloat("_Gradient", 1f, skinnedMeshRenderer);
				ShaderEffects.SetColor("_GradientColor", ShaderConstants.GRADIENT_OUTLINE, skinnedMeshRenderer);
			}
		}
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_BANISH.ToString()));
		base.BoostAbilityEmission();
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
