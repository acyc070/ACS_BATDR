using System;
using S13Audio.BATDR;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWeaponGentPipe : PlayerWeaponState
{
	private bool m_IsCharged;

	private bool m_IsOvercharged;

	private static Shader FPSToonShader;

	private static Shader FPSParticleShader;

	private Renderer[] renderers;

	private Renderer[] particleRenderers;

	public override bool IsCharged => this.m_IsCharged;

	public override bool IsOvercharged => this.m_IsOvercharged;

	public PlayerWeaponGentPipe(Player player, WeaponData data)
		: base(player, data)
	{
	}

	public override void OnInitialize()
	{
		if (base.m_Weapon == null)
		{
			base.m_Weapon = GameManager.Instance.AssetManager.CreateAsset<Weapon>("Weapons/Weapon_GentPipe", base.Data);
			base.m_Weapon.SetParentAndCenter(base.Actor.WeaponParent);
		}
		else
		{
			base.m_Weapon.gameObject.SetActive(value: true);
		}
		this.renderers = base.m_Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		this.particleRenderers = base.m_Weapon.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		base.SetActive(active: true);
	}

	public void SwapShaderToFirstPerson()
	{
		Renderer[] array;
		array = this.renderers;
		foreach (Renderer obj in array)
		{
			obj.material.shader = PlayerWeaponGentPipe.FPSToonShader;
			obj.shadowCastingMode = ShadowCastingMode.Off;
		}
		array = this.particleRenderers;
		unchecked
		{
			for (int i = 0; i < array.Length; i++)
			{
				Material[] materials;
				materials = array[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].shader = PlayerWeaponGentPipe.FPSParticleShader;
				}
			}
		}
	}

	public void SetFirstPersonFlag(bool active)
	{
		unchecked
		{
			Renderer[] array;
			if (active)
			{
				array = this.renderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].material.EnableKeyword("FIRST_PERSON");
				}
				array = this.particleRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					Material[] materials;
					materials = array[i].materials;
					for (int j = 0; j < materials.Length; j++)
					{
						materials[j].EnableKeyword("FIRST_PERSON");
					}
				}
				return;
			}
			array = this.renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material.DisableKeyword("FIRST_PERSON");
			}
			array = this.particleRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				Material[] materials;
				materials = array[i].materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].DisableKeyword("FIRST_PERSON");
				}
			}
		}
	}

	protected override bool InternalCheckOvercharge()
	{
		if (this.m_IsOvercharged || !this.m_IsCharged || base.Data.WeaponType != WeaponType.LEVEL_4)
		{
			return false;
		}
		bool result;
		result = false;
		GentPipePowerIndicator componentInChildren;
		componentInChildren = base.m_Weapon.GetComponentInChildren<GentPipePowerIndicator>();
		if (componentInChildren != null && componentInChildren.Power >= 1)
		{
			result = true;
		}
		return result;
	}

	protected override bool InternalOvercharge()
	{
		GentPipePowerIndicator componentInChildren;
		componentInChildren = base.Actor.WeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			this.SetPowerIndicator(componentInChildren, base.Actor.WeaponParent, hasListeners: true, 1);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(componentInChildren.Power);
		}
		this.SetPowerIndicator(base.Actor.BodyWeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.BodyWeaponParent, hasListeners: false, 1);
		this.m_IsOvercharged = true;
		this.OverchargeShake();
		return true;
	}

	protected override bool InternalCheckCharge()
	{
		if (this.m_IsCharged || base.Data.WeaponType == WeaponType.NONE || base.Data.WeaponType == WeaponType.LEVEL_1 || base.Data.WeaponType == WeaponType.LEVEL_2)
		{
			return false;
		}
		bool result;
		result = false;
		GentPipePowerIndicator componentInChildren;
		componentInChildren = base.m_Weapon.GetComponentInChildren<GentPipePowerIndicator>();
		if (componentInChildren != null && componentInChildren.Power >= 2)
		{
			result = true;
		}
		return result;
	}

	protected override bool InternalCharge()
	{
		GentPipePowerIndicator componentInChildren;
		componentInChildren = base.Actor.WeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			this.SetPowerIndicator(componentInChildren, base.Actor.WeaponParent, hasListeners: true, 2);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(componentInChildren.Power);
		}
		this.SetPowerIndicator(base.Actor.ActiveWeaponDuplicate.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.ActiveWeaponDuplicate.transform, hasListeners: false, 2);
		this.SetPowerIndicator(base.Actor.ActiveWeaponMirrorDuplicate.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true), base.Actor.ActiveWeaponMirrorDuplicate.transform, hasListeners: false, 2);
		this.m_IsCharged = true;
		this.ChargeShake();
		return true;
	}

	private void SetPowerIndicator(GentPipePowerIndicator powerIndicator, Transform weaponParent, bool hasListeners, int powerAmount)
	{
		if (!(powerIndicator != null))
		{
			return;
		}
		int num;
		num = unchecked(powerIndicator.Power - powerAmount);
		if (num <= 0)
		{
			num = 0;
		}
		powerIndicator.SetPowerLevel(num);
		GentPipeChargeEffects componentInChildren;
		componentInChildren = weaponParent.GetComponentInChildren<GentPipeChargeEffects>();
		if (componentInChildren != null)
		{
			if (hasListeners)
			{
				componentInChildren.OnEnd -= this.HandleChargedEffectsOnEnd;
				componentInChildren.OnEnd += this.HandleChargedEffectsOnEnd;
			}
			componentInChildren.Enable();
		}
	}

	private void HandleChargedEffectsOnEnd(object sender, EventArgs e)
	{
		(sender as GentPipeChargeEffects).OnEnd -= this.HandleChargedEffectsOnEnd;
		this.m_IsCharged = false;
		this.m_IsOvercharged = false;
	}

	protected override bool InternalCheckUse()
	{
		return true;
	}

	protected override bool InternalUse()
	{
		return true;
	}

	protected override void InternalOnAttack()
	{
		Transform transform;
		transform = GameManager.Instance.GameCamera.transform;
		Vector3 origin;
		origin = transform.position - transform.forward;
		unchecked
		{
			if (Physics.SphereCast(origin, this.GetAttackRadius(), transform.forward, out var hitInfo, 9f, LayerMaskUtility.GetWeaponAttack, QueryTriggerInteraction.Ignore))
			{
				bool flag;
				flag = false;
				IHittable componentInParent;
				componentInParent = hitInfo.transform.gameObject.GetComponentInParent<IHittable>();
				if (componentInParent == null)
				{
					Character component;
					component = hitInfo.transform.gameObject.GetComponent<Character>();
					int num;
					num = 0;
					if (component != null)
					{
						this.HitShake();
						component.OnHit(hitInfo);
						Collider[] array;
						array = Physics.OverlapSphere(hitInfo.point, 2f, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore);
						for (int i = 0; i < array.Length; i++)
						{
							if (num > 2)
							{
								break;
							}
							Collider collider;
							collider = array[i];
							if (collider.gameObject.layer == LayerMask.NameToLayer("AI") || collider.gameObject.layer == LayerMask.NameToLayer("IgnoreAI"))
							{
								Character component2;
								component2 = collider.gameObject.GetComponent<Character>();
								if (component2 != null && component2 != component)
								{
									component2.OnHit(hitInfo);
									num++;
								}
							}
						}
					}
					else
					{
						flag = true;
					}
				}
				else if (componentInParent.IsPlayerBreakable && !componentInParent.IsBroken)
				{
					this.HitShake();
					componentInParent.Hit(hitInfo);
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (Physics.SphereCast(origin, 0.1f, transform.forward, out var hitInfo2, 9f, LayerMaskUtility.GetWeaponAttack, QueryTriggerInteraction.Ignore))
					{
						this.GetHitDecal(hitInfo2);
					}
					else
					{
						this.MissShake();
					}
				}
			}
			else
			{
				this.MissShake();
			}
		}
	}

	private float GetAttackRadius()
	{
		float result;
		result = 0.5f;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 0.75f;
			break;
		case DifficultyLevel.Hard:
			result = 0.25f;
			break;
		case DifficultyLevel.Impossible:
			result = 0.1f;
			break;
		}
		return result;
	}

	private void MissShake()
	{
		CameraEffects.ShakeRotation(0.2f, 2f, 5, 90f, fadeOut: false);
	}

	private void HitShake()
	{
		CameraEffects.ShakeRotation(0.2f, 4f, 10, 90f, fadeOut: false);
	}

	private void ChargeShake()
	{
		CameraEffects.ShakeRotation(0.5f, 5f, 15);
	}

	private void OverchargeShake()
	{
		CameraEffects.ShakeRotation(0.75f, 10f, 20);
	}

	private void GetHitDecal(RaycastHit hit)
	{
		string text;
		text = "Impacts/Impact_Generic";
		FloorMaterial floor;
		if (this.CheckImpactMaterial(hit, out var impact))
		{
			text = this.GetImpact(impact);
		}
		else if (this.CheckFloorMaterial(hit, out floor))
		{
			text = this.GetFloor(floor);
		}
		if (text != string.Empty)
		{
			GameManager.Instance.PoolingManager.GetFromPool(text, 4f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		}
		this.HitShake();
		if (this.IsCharged)
		{
			text = "Impacts/Impact_Electric";
			GentPipeChargeEffects componentInChildren;
			componentInChildren = base.m_Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				componentInChildren.Hit();
			}
			GameManager.Instance.PoolingManager.GetFromPool(text, 4f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		}
	}

	private bool CheckImpactMaterial(RaycastHit hit, out ImpactMaterial impact)
	{
		bool result;
		result = true;
		impact = hit.transform.gameObject.GetComponent<ImpactMaterial>();
		if (impact == null)
		{
			result = false;
		}
		return result;
	}

	private string GetImpact(ImpactMaterial impactMaterial)
	{
		string result;
		result = "Impacts/Impact_Generic";
		if (impactMaterial.ImpactType == ImpactType.Dirt)
		{
			result = "Impacts/Impact_Dirt";
		}
		else if (impactMaterial.ImpactType == ImpactType.Paper)
		{
			result = "Impacts/Impact_Paper";
		}
		else if (impactMaterial.ImpactType == ImpactType.Brick)
		{
			result = "Impacts/Impact_Brick";
		}
		else if (impactMaterial.ImpactType == ImpactType.Fabric)
		{
			result = "Impacts/Impact_Fabric";
		}
		else if (impactMaterial.ImpactType == ImpactType.Metal)
		{
			result = "Impacts/Impact_Metal";
		}
		else if (impactMaterial.ImpactType == ImpactType.Spark)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (impactMaterial.ImpactType == ImpactType.Stone)
		{
			result = "Impacts/Impact_Stone";
		}
		else if (impactMaterial.ImpactType == ImpactType.Tile)
		{
			result = "Impacts/Impact_Tile";
		}
		else if (impactMaterial.ImpactType == ImpactType.Wood)
		{
			result = "Impacts/Impact_Wood";
		}
		else if (impactMaterial.ImpactType == ImpactType.Squeak)
		{
			result = "Impacts/Impact_Squeak";
		}
		else if (impactMaterial.ImpactType == ImpactType.Generic)
		{
			result = "Impacts/Impact_Generic";
		}
		else if (impactMaterial.ImpactType == ImpactType.Ink)
		{
			result = "Impacts/Impact_Ink";
		}
		else if (impactMaterial.ImpactType == ImpactType.InkColor)
		{
			result = "Impacts/Impact_Ink_Color";
		}
		else if (impactMaterial.ImpactType == ImpactType.NONE)
		{
			result = string.Empty;
		}
		return result;
	}

	private bool CheckFloorMaterial(RaycastHit hit, out FloorMaterial floor)
	{
		bool result;
		result = true;
		floor = hit.transform.gameObject.GetComponent<FloorMaterial>();
		if (floor == null)
		{
			result = false;
		}
		return result;
	}

	private string GetFloor(FloorMaterial floorMaterial)
	{
		string result;
		result = "Impacts/Impact_Generic";
		if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Metal)
		{
			result = "Impacts/Impact_Metal";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Stone)
		{
			result = "Impacts/Impact_Stone";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Tile)
		{
			result = "Impacts/Impact_Tile";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Wood)
		{
			result = "Impacts/Impact_Wood";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Vent)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.Pipe)
		{
			result = "Impacts/Impact_Sparks";
		}
		else if (floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkDeep || floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkPuddle || floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkShallow)
		{
			result = "Impacts/Impact_Ink";
		}
		return result;
	}

	protected override void InternalOnAttackComplete()
	{
		base.SetActive(active: true);
	}

	protected override void InternalReset()
	{
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalOnStateExit()
	{
		if (base.m_Weapon != null)
		{
			base.m_Weapon.gameObject.SetActive(value: false);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}

	static PlayerWeaponGentPipe()
	{
		PlayerWeaponGentPipe.FPSToonShader = Shader.Find("JDS/FPS Toon Character Shader");
		PlayerWeaponGentPipe.FPSParticleShader = Shader.Find("JDS/Particles/FPS Standard Unlit");
	}
}
