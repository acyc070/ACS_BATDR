using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class TimelineCutsceneWeaponBehaviour : PlayableBehaviour
{
	public Transform Target;

	public bool UseData;

	public bool UsePower;

	public int PowerCost;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (!Application.isPlaying || this.m_IsPlayed)
		{
			return;
		}
		this.m_IsPlayed = true;
		unchecked
		{
			if (this.UsePower)
			{
				this.SetPowerIndicator(this.Target.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true));
				this.SetPowerIndicator(GameManager.Instance.Player.WeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true));
				this.SetPowerIndicator(GameManager.Instance.Player.BodyWeaponParent.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true));
				int num;
				num = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power - this.PowerCost;
				if (num <= 0)
				{
					num = 0;
				}
				GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(num);
			}
			else if (this.UseData)
			{
				this.GetWeapon();
			}
			else if (GameManager.Instance.Player.ActiveWeaponDuplicate != null)
			{
				GameObject gameObject;
				gameObject = Object.Instantiate(GameManager.Instance.Player.ActiveWeaponDuplicate);
				gameObject.transform.SetParent(this.Target);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
				gameObject.transform.localScale = GameManager.Instance.Player.ActiveWeaponDuplicate.transform.localScale;
				MeshRenderer[] componentsInChildren;
				componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
				}
			}
		}
	}

	private void SetPowerIndicator(GentPipePowerIndicator powerIndicator)
	{
		if (powerIndicator != null)
		{
			powerIndicator.SetPowerLevel(0);
		}
	}

	private void GetWeapon()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID != WeaponType.NONE)
		{
			WeaponType iD;
			iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
			string assetKey;
			assetKey = "";
			switch (iD)
			{
			case WeaponType.LEVEL_1:
				assetKey = "Weapons/Weapon_GentPipe_01";
				break;
			case WeaponType.LEVEL_2:
				assetKey = "Weapons/Weapon_GentPipe_02";
				break;
			case WeaponType.LEVEL_3:
				assetKey = "Weapons/Weapon_GentPipe_03";
				break;
			case WeaponType.LEVEL_4:
				assetKey = "Weapons/Weapon_GentPipe_04";
				break;
			}
			Transform transform;
			transform = GameManager.Instance.AssetManager.CreateAsset<Transform>(assetKey);
			MeshRenderer[] componentsInChildren;
			componentsInChildren = transform.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			foreach (MeshRenderer obj in componentsInChildren)
			{
				obj.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
				obj.shadowCastingMode = ShadowCastingMode.On;
			}
			this.InitWeaponParent(this.Target, transform.gameObject);
		}
	}

	private void InitWeaponParent(Transform weaponParent, GameObject weapon)
	{
		weapon.transform.SetParent(weaponParent);
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localEulerAngles = Vector3.zero;
		weapon.transform.localScale = Vector3.one;
		GentPipePowerIndicator componentInChildren;
		componentInChildren = weapon.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
		}
		if (GameManager.Instance.Player.CurrentWeapon == null)
		{
			return;
		}
		GentPipeChargeEffects componentInChildren2;
		componentInChildren2 = weapon.GetComponentInChildren<GentPipeChargeEffects>(includeInactive: true);
		if (componentInChildren2 != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			componentInChildren2.ChargeLevel.gameObject.SetActive(value: true);
			GentPipeChargeEffects componentInChildren3;
			componentInChildren3 = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>(includeInactive: true);
			if (componentInChildren3 != null)
			{
				componentInChildren2.ChargeLevel.localScale = componentInChildren3.ChargeLevel.localScale;
			}
		}
	}
}
