using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public abstract class PlayerAbilityState : AbilityState<Player, State.PlayerAbility>
{
	protected const string POWER_SHADER_TEXTURE = "_Emission";

	protected const string POWER_SHADER_EMISSION = "_EmitPower";

	protected const string POWER_SHADER_EMISSION_COLOR = "_EmissionColor";

	protected const float POWER_EMISSION_OFF = 0f;

	protected const float POWER_EMISSION = 0.7f;

	protected const float POWER_EMISSION_BOOST = 1.3f;

	protected const float POWER_EMISSION_USE = 2f;

	protected Color POWER_EMISSION_COLOR = new Color32(120, 60, 5, byte.MaxValue);

	protected const float POST_PROCESS_EFFECT_OFF = 0f;

	protected const float POST_PROCESS_EFFECT_MAX = 1f;

	protected Texture2D m_Sigil;

	protected Ability m_AbilityPostProcessEffect;

	protected float m_AbilityUseDelay;

	protected float m_AbilityUseDuration = 0.5f;

	protected float m_AbilityDistanceMin;

	protected float m_AbilityDistanceMax;

	protected float m_Cooldown;

	private Sequence m_HandMaterialSequence;

	protected Color POWER_EMISSION_COLOR_BOOST => this.POWER_EMISSION_COLOR * 1.25f;

	protected Color POWER_EMISSION_COLOR_USE => this.POWER_EMISSION_COLOR * 2f;

	protected Material m_HandMaterial => base.Actor.HandMaterial;

	public float _Cooldown => this.m_Cooldown;

	protected virtual float m_CooldownCount { get; set; }

	public float _CooldownCount => this.m_CooldownCount;

	public bool IsReady { get; protected set; }

	public bool CanUse { get; protected set; }

	public bool IsUsed { get; protected set; }

	public bool IsBoosting { get; private set; }

	public bool IsCooldown { get; protected set; }

	protected virtual bool UseInput()
	{
		return false;
	}

	public PlayerAbilityState(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	protected sealed override void InternalOnStateEnter()
	{
		this.InternalAbilityInitializer();
		base.SetActive(active: false);
		this.m_CooldownCount = this.m_Cooldown;
		this.IsCooldown = false;
		this.ResetHandSequence();
		this.m_HandMaterialSequence.OnComplete(this.StateEnterOnComplete);
	}

	public abstract void InternalAbilityInitializer();

	private void StateEnterOnComplete()
	{
		base.SetActive(active: true);
		GameManager.Instance.ShowCrosshair();
		this.ShowAbility();
		this.InternalStateEnterOnComplete();
	}

	protected virtual void InternalStateEnterOnComplete()
	{
	}

	protected override void InternalUpdateInput()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
		{
			return;
		}
		if (this.IsCooldown && this.m_Cooldown <= this.m_CooldownCount)
		{
			this.m_Cooldown += Time.deltaTime;
			this.CheckCooldown();
			if (this.m_Cooldown >= this.m_CooldownCount)
			{
				this.m_Cooldown = this.m_CooldownCount;
				this.IsCooldown = false;
				this.CooldownComplete();
			}
		}
		if ((base.Actor.CombatStatus != CombatStatus.Hide || base.Actor.CharacterController.enabled) && base.Actor.CurrentState == State.Player.Default && this.m_Cooldown >= this.m_CooldownCount && !base.Actor.IsAbilityLocked && this.IsReady && this.IsUsed && this.CanUse && this.UseInput())
		{
			this.UseAbility();
		}
	}

	protected virtual void CheckCooldown()
	{
	}

	protected virtual void CooldownComplete()
	{
	}

	public void ForceCooldown()
	{
		this.ForceSetCooldown(0f);
		this.IsCooldown = true;
	}

	public void ForceSetCooldown(float cooldown)
	{
		this.m_Cooldown = cooldown;
		if (this.m_Cooldown > this.m_CooldownCount)
		{
			this.m_Cooldown = this.m_CooldownCount;
		}
	}

	private void ShowAbility()
	{
		this.IsUsed = true;
		base.SetActive(active: false);
		this.ShowSigil();
		this.IsReady = true;
		base.SetActive(active: true);
	}

	private void ShowSigil()
	{
		this.ResetHandSequence();
		float atPosition;
		atPosition = 0f;
		float duration;
		duration = 0f;
		this.m_HandMaterial.SetTexture("_Emission", this.m_Sigil);
		this.m_HandMaterialSequence.Insert(atPosition, this.DOEmission("_EmitPower", 0.7f, duration, Ease.OutSine));
		this.m_HandMaterialSequence.Insert(atPosition, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR, duration, Ease.OutSine));
	}

	private void UseAbility()
	{
		if (this.Use())
		{
			base.Actor.Ability();
			this.IsCooldown = true;
			this.m_Cooldown = 0f;
		}
		else
		{
			this.IsCooldown = false;
			this.m_Cooldown = this.m_CooldownCount;
		}
		this.IsUsed = false;
		this.IsReady = false;
	}

	protected sealed override bool CheckReady()
	{
		if (base.Actor.CurrentState != State.Player.Default)
		{
			this.CanUse = false;
			return false;
		}
		if (!this.IsReady)
		{
			this.CanUse = false;
			this.InternalNotReady();
		}
		return this.IsReady;
	}

	protected virtual void InternalNotReady()
	{
	}

	protected sealed override bool CheckUse()
	{
		return this.InternalCheckUse();
	}

	protected abstract bool InternalCheckUse();

	public sealed override bool Use()
	{
		if (this.CheckUse())
		{
			base.SetActive(active: false);
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.ClearInteraction();
			this.ResetPostProcessEffect();
			this.ResetHandSequence();
			this.m_HandMaterial.SetTexture("_Emission", this.m_Sigil);
			this.m_HandMaterial.SetFloat("_EmitPower", 0.7f);
			this.m_HandMaterial.SetColor("_EmissionColor", this.POWER_EMISSION_COLOR);
			float abilityUseDelay;
			abilityUseDelay = this.m_AbilityUseDelay;
			float duration;
			duration = 0.1f;
			this.m_HandMaterialSequence.Insert(abilityUseDelay, this.DOEmission("_EmitPower", 2f, duration, Ease.InSine));
			this.m_HandMaterialSequence.Insert(abilityUseDelay, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR_USE, duration, Ease.InSine));
			abilityUseDelay += 0.1f;
			duration = this.m_AbilityUseDuration;
			this.m_HandMaterialSequence.Insert(abilityUseDelay, this.DOEmission("_EmitPower", 0f, duration, Ease.InSine));
			this.m_HandMaterialSequence.Insert(abilityUseDelay, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR, duration, Ease.InSine));
			return this.InternalUseClearCheck();
		}
		return false;
	}

	private bool InternalUseClearCheck()
	{
		bool result;
		result = this.InternalUse();
		this.ClearCheck();
		return result;
	}

	protected abstract bool InternalUse();

	protected void UseOnComplete()
	{
		this.InternalUseOnComplete();
		this.ResetPostProcessEffect(active: false);
		this.Reset();
	}

	protected virtual void InternalUseOnComplete()
	{
	}

	public sealed override void Reset()
	{
		this.ResetHandSequence();
		float atPosition;
		atPosition = 0f;
		float duration;
		duration = 0.4f;
		this.m_HandMaterialSequence.Insert(atPosition, this.DOEmission("_EmitPower", 0f, duration, Ease.InSine));
		this.m_HandMaterialSequence.Insert(atPosition, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR, duration, Ease.InSine));
		this.m_HandMaterialSequence.OnComplete(this.RestOnComplete);
	}

	private void RestOnComplete()
	{
		base.Actor.HideAbility(this.HideAbilityOnComplete);
		this.ShowAbility();
		this.InternalResetOnComplete();
	}

	protected virtual void InternalResetOnComplete()
	{
	}

	private void HideAbilityOnComplete()
	{
		base.SetActive(active: true);
	}

	protected void BoostAbilityEmission()
	{
		if (!this.IsBoosting)
		{
			this.IsBoosting = true;
			this.ResetHandSequence();
			float atPosition;
			atPosition = 0f;
			float duration;
			duration = 0.4f;
			this.m_HandMaterialSequence.Insert(atPosition, this.DOEmission("_EmitPower", 1.3f, duration, Ease.InOutSine));
			this.m_HandMaterialSequence.Insert(atPosition, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR_BOOST, duration, Ease.InOutSine));
		}
	}

	protected void ResetAbilityEmission()
	{
		if (this.IsBoosting)
		{
			this.IsBoosting = false;
			this.ResetHandSequence();
			float atPosition;
			atPosition = 0f;
			float duration;
			duration = 0.4f;
			this.m_HandMaterialSequence.Insert(atPosition, this.DOEmission("_EmitPower", 0.7f, duration, Ease.InOutSine));
			this.m_HandMaterialSequence.Insert(atPosition, this.DOColor("_EmissionColor", this.POWER_EMISSION_COLOR, duration, Ease.InOutSine));
		}
	}

	protected void DOKillHandMaterial()
	{
		this.m_HandMaterial.DOKill();
	}

	protected Tween DOEmission(string name, float endValue, float duration, Ease ease)
	{
		return this.m_HandMaterial.DOFloat(endValue, name, duration).SetEase(ease);
	}

	protected Tween DOColor(string name, Color endValue, float duration, Ease ease)
	{
		return this.m_HandMaterial.DOColor(endValue, name, duration).SetEase(ease);
	}

	protected Tween DOPostProcess(float endValue, float duration, Ease ease)
	{
		int hashCode;
		hashCode = this.m_AbilityPostProcessEffect._Power.value.GetHashCode();
		DOTween.Kill(hashCode);
		return DOTween.To(() => this.m_AbilityPostProcessEffect._Power.value, (float x) =>
		{
			this.m_AbilityPostProcessEffect._Power.value = x;
		}, endValue, duration).SetEase(ease).SetId(hashCode);
	}

	public void ResetReady()
	{
		this.IsReady = false;
		this.IsUsed = false;
	}

	protected void ResetPostProcessEffect(bool active = true)
	{
		if (this.m_AbilityPostProcessEffect != null)
		{
			this.m_AbilityPostProcessEffect.active = active;
			this.m_AbilityPostProcessEffect._Power.value = 0f;
		}
	}

	public void ForceCancel()
	{
		this.IsCooldown = false;
		this.m_Cooldown = this.m_CooldownCount;
		if (this.m_AbilityPostProcessEffect != null && this.m_AbilityPostProcessEffect._Power != null)
		{
			this.m_AbilityPostProcessEffect._Power.value = 0f;
		}
		this.Reset();
		this.InternalForceCancel();
	}

	protected virtual void InternalForceCancel()
	{
	}

	private void ResetHandSequence()
	{
		this.KillHandSequence();
		this.DOKillHandMaterial();
		this.m_HandMaterialSequence = DOTween.Sequence();
	}

	private void KillHandSequence()
	{
		if (this.m_HandMaterialSequence != null)
		{
			this.m_HandMaterialSequence.Kill();
			this.m_HandMaterialSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillHandSequence();
		this.DOKillHandMaterial();
		base.OnDisposed();
	}
}
