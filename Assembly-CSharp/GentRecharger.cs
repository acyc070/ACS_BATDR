using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentRecharger : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_InteractableBatteries;

	[SerializeField]
	private InteractableAnimationBase m_InteractableCharger;

	[Header("Lighting")]
	[SerializeField]
	private Renderer m_EmissionRenderer;

	[SerializeField]
	private GameObject m_Lighting;

	[Header("Temp")]
	[SerializeField]
	private GameObject m_Electricity;

	[SerializeField]
	private Renderer m_ChargerEmissions;

	[SerializeField]
	private GameObject[] m_PowerLevels;

	[SerializeField]
	private GameObject[] m_Batteries;

	private Sequence m_BatterySequence;

	private Sequence m_ChargerSequence;

	private GameObject m_Weapon;

	private bool m_DisableGentRecharger;

	private bool m_IsBatteryActive;

	private bool m_IsChargerActive;

	private int m_BatteryMax = 5;

	private int m_BatteryCount;

	public InteractableAnimationBase InteractableBatteries => this.m_InteractableBatteries;

	public InteractableAnimationBase InteractableCharger => this.m_InteractableCharger;

	public bool IsActive { get; protected set; }

	public int BatteryCount => this.m_BatteryCount;

	public event EventHandler OnDeposit;

	public event EventHandler OnBatteriesComplete;

	public event EventHandler OnCharged;

	public void Initialize(int batteries)
	{
		this.m_BatteryCount = batteries;
		unchecked
		{
			for (int i = 0; i < this.m_Batteries.Length; i++)
			{
				GameObject gameObject;
				gameObject = this.m_Batteries[i];
				if (i >= this.m_BatteryCount)
				{
					gameObject.SetActive(value: false);
				}
				else
				{
					gameObject.SetActive(value: true);
				}
			}
			for (int j = 0; j < this.m_PowerLevels.Length; j++)
			{
				this.m_PowerLevels[j].SetActive(value: false);
			}
			this.m_ChargerEmissions.material.SetFloat("_Power", 0f);
			this.m_Electricity.SetActive(value: false);
			this.SetActive(active: false);
			this.RemoveListeners();
			this.m_InteractableBatteries.OnInteract += this.HandleInteractableBatteriesOnInteract;
			this.m_InteractableBatteries.OnInteractionComplete += this.HandleInteractableBatteriesOnInteractionComplete;
			this.m_InteractableBatteries.SetActive(active: false);
			this.m_InteractableCharger.OnInteract += this.HandleInteractableCharacterOnInteract;
			this.m_InteractableCharger.OnInteractionComplete += this.HandleInteractableCharacterOnInteractionComplete;
			this.m_InteractableCharger.OnInteractionExit += this.HandleInteractableCharacterOnInteractionExit;
			this.m_InteractableCharger.SetActive(active: false);
			if (this.m_BatteryCount >= this.m_BatteryMax)
			{
				this.m_IsBatteryActive = true;
				this.m_IsChargerActive = true;
				this.SetActive(active: true);
			}
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!this.m_DisableGentRecharger)
		{
			if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
			{
				if (this.m_InteractableBatteries.IsActive)
				{
					this.m_InteractableBatteries.ForceDisable();
				}
				if (this.m_InteractableCharger.IsActive)
				{
					this.m_InteractableCharger.ForceDisable();
				}
				return;
			}
			if (this.m_IsBatteryActive && !this.m_IsChargerActive && !this.m_InteractableBatteries.IsActive)
			{
				this.CheckBatteryDeposit();
			}
			if (!this.m_IsBatteryActive && !this.m_IsChargerActive && !this.m_InteractableBatteries.IsActive)
			{
				this.CheckBatteryDeposit();
			}
			else if (this.m_IsBatteryActive && this.m_IsChargerActive && !this.m_InteractableCharger.IsActive && !this.m_InteractableCharger.IsActive && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.LEVEL_1 && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.NONE && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power < 6)
			{
				this.m_InteractableCharger.SetActive(active: true);
				this.m_InteractableCharger.ResetAction();
			}
		}
		else
		{
			if (this.m_InteractableBatteries.IsActive)
			{
				this.m_InteractableBatteries.ForceDisable();
			}
			if (this.m_InteractableCharger.IsActive)
			{
				this.m_InteractableCharger.ForceDisable();
			}
		}
	}

	public void SetGentRecharger(bool active)
	{
		this.m_DisableGentRecharger = !active;
	}

	private void CheckBatteryDeposit()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries > 0 && this.m_BatteryCount < this.m_BatteryMax)
		{
			this.m_IsBatteryActive = true;
			this.m_InteractableBatteries.SetActive(active: true);
			this.m_InteractableBatteries.ResetAction();
		}
	}

	private void HandleInteractableCharacterOnInteractionExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
		if (this.m_Weapon != null)
		{
			global::UnityEngine.Object.Destroy(this.m_Weapon);
			this.m_Weapon = null;
		}
	}

	private void HandleInteractableCharacterOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.ActiveWeaponDuplicate != null)
		{
			GameManager.Instance.Player.SetCollision(active: false);
			this.m_Weapon = global::UnityEngine.Object.Instantiate(GameManager.Instance.Player.ActiveWeaponDuplicate);
			this.m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent);
			this.m_Weapon.transform.localPosition = Vector3.zero;
			this.m_Weapon.transform.localEulerAngles = Vector3.zero;
			this.m_Weapon.transform.localScale = GameManager.Instance.Player.ActiveWeaponDuplicate.transform.localScale;
			MeshRenderer[] componentsInChildren;
			componentsInChildren = this.m_Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
			{
				componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
			}
			this.m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent.parent);
		}
	}

	private void HandleInteractableBatteriesOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: false);
		this.m_InteractableBatteries.SetActive(active: false);
		int batteries;
		batteries = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries;
		unchecked
		{
			for (int i = 0; i < batteries; i++)
			{
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(1);
				this.m_BatteryCount++;
				if (this.m_BatteryCount >= this.m_BatteryMax)
				{
					break;
				}
			}
			this.ResetBatterySequence();
			float num;
			num = 2f;
			for (int j = 0; j < this.m_Batteries.Length; j++)
			{
				if (j >= this.m_BatteryCount)
				{
					break;
				}
				GameObject batteries2;
				batteries2 = this.m_Batteries[j];
				num += (float)j * 0.03f;
				this.m_BatterySequence.InsertCallback(num, () =>
				{
					batteries2.SetActive(value: true);
				});
			}
			this.m_BatterySequence.InsertCallback(num + 0.25f, this.BatteriesOnComplete);
		}
	}

	private void BatteriesOnComplete()
	{
		this.OnDeposit.Send(this);
		if (this.m_BatteryCount >= this.m_BatteryMax)
		{
			this.m_IsChargerActive = true;
			this.SetActive(active: true);
			this.OnBatteriesComplete.Send(this);
		}
		else
		{
			this.m_IsBatteryActive = false;
		}
	}

	private void HandleInteractableBatteriesOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
	}

	private void HandleInteractableCharacterOnInteractionComplete(object sender, EventArgs e)
	{
		this.m_ChargerEmissions.material.DOFloat(100f, "_Power", 3f);
		this.ResetChargerSequence();
		float num;
		num = 1.2f;
		float num2;
		num2 = num - 0.1f;
		this.m_ChargerSequence.InsertCallback(1f, () =>
		{
			this.m_Electricity.SetActive(value: true);
		});
		unchecked
		{
			for (int num3 = this.m_Batteries.Length - 1; num3 >= 0; num3--)
			{
				GameObject batteries;
				batteries = this.m_Batteries[num3];
				num2 += (float)num3 * 0.05f;
				this.m_ChargerSequence.InsertCallback(num2, () =>
				{
					batteries.SetActive(value: false);
				});
			}
			for (int num4 = 0; num4 < this.m_PowerLevels.Length; num4++)
			{
				GameObject powerlevel;
				powerlevel = this.m_PowerLevels[num4];
				num += (float)num4 * 0.045f;
				this.m_ChargerSequence.InsertCallback(num, () =>
				{
					powerlevel.SetActive(value: true);
				});
			}
			this.m_ChargerSequence.InsertCallback(num + 0.1f, this.ShutOff);
		}
	}

	private void ShutOff()
	{
		this.m_InteractableCharger.SetActive(active: false);
		this.m_InteractableCharger.Exit();
		unchecked
		{
			for (int i = 0; i < this.m_PowerLevels.Length; i++)
			{
				this.m_PowerLevels[i].SetActive(value: false);
			}
			this.m_ChargerEmissions.material.SetFloat("_Power", 0f);
			this.m_Electricity.SetActive(value: false);
			this.SetActive(active: false);
			this.m_BatteryCount = 0;
			this.m_IsBatteryActive = false;
			this.m_IsChargerActive = false;
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
			GentPipePowerIndicator[] componentsInChildren;
			componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SetPowerLevel(6);
			}
			this.OnCharged.Send(this);
		}
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
		float value;
		value = (this.IsActive ? 1f : 0f);
		this.m_EmissionRenderer.material.SetFloat("_Power", value);
		this.m_Lighting.SetActive(this.IsActive);
	}

	private void ResetBatterySequence()
	{
		this.KillBatterySequence();
		this.m_BatterySequence = DOTween.Sequence();
	}

	private void KillBatterySequence()
	{
		if (this.m_BatterySequence != null)
		{
			this.m_BatterySequence.Kill();
			this.m_BatterySequence = null;
		}
	}

	private void ResetChargerSequence()
	{
		this.KillChargerSequence();
		this.m_ChargerSequence = DOTween.Sequence();
	}

	private void KillChargerSequence()
	{
		if (this.m_ChargerSequence != null)
		{
			this.m_ChargerSequence.Kill();
			this.m_ChargerSequence = null;
		}
	}

	private void RemoveListeners()
	{
		this.m_InteractableCharger.OnInteract -= this.HandleInteractableCharacterOnInteract;
		this.m_InteractableBatteries.OnInteractionComplete -= this.HandleInteractableBatteriesOnInteractionComplete;
		this.m_InteractableBatteries.OnInteract -= this.HandleInteractableBatteriesOnInteract;
		this.m_InteractableCharger.OnInteractionComplete -= this.HandleInteractableCharacterOnInteractionComplete;
		this.m_InteractableCharger.OnInteractionExit -= this.HandleInteractableCharacterOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.OnDeposit = null;
		this.OnBatteriesComplete = null;
		this.OnCharged = null;
		this.m_Weapon = null;
		this.KillBatterySequence();
		this.KillChargerSequence();
		this.RemoveListeners();
		base.OnDisposed();
	}
}
