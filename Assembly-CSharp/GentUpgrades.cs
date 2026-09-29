using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentUpgrades : JMonoBehaviour
{
	[Header("Interaction")]
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	[SerializeField]
	private MeshOutline m_MeshOutline;

	[Header("Lighting")]
	[SerializeField]
	private Renderer m_EmissionRenderer;

	[SerializeField]
	private GameObject m_Lighting;

	[SerializeField]
	private GameObject m_Particles;

	[Header("Prompts")]
	[SerializeField]
	private Sprite m_Prompt_LV3;

	[SerializeField]
	private Sprite m_Prompt_LV4;

	private GameObject m_Weapon;

	private Sequence m_Sequence;

	private bool m_WasUpgraded;

	public bool IsActive { get; protected set; }

	public event EventHandler OnUpgrade;

	public override void Start()
	{
		this.m_Particles.SetActive(value: false);
		this.m_MeshOutline.SetActive(active: false);
		this.SetActive(active: false);
		this.m_Interactable.SetActive(active: false);
		this.RemoveListeners();
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteractionComplete += this.HandleInteractableOnInteractionComplete;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			if (this.m_Interactable.IsActive)
			{
				this.m_Interactable.ForceDisable();
			}
			return;
		}
		if (this.IsActive && !this.m_Interactable.IsActive)
		{
			this.m_Interactable.SetActive(active: true);
			this.m_Interactable.ResetAction();
		}
		if (GameManager.Instance.Player.CurrentWeapon != null)
		{
			WeaponType iD;
			iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
			if (!this.IsActive)
			{
				this.CheckEnable();
				return;
			}
			int count;
			count = GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Count;
			if (count > 0)
			{
				if (iD == WeaponType.LEVEL_4)
				{
					this.Disable();
				}
				else if (count < 3 && iD == WeaponType.LEVEL_3)
				{
					this.Disable();
				}
				else if (count < 2 && iD == WeaponType.LEVEL_2)
				{
					this.Disable();
				}
			}
		}
		else if (this.IsActive)
		{
			this.Disable();
		}
	}

	private void CheckEnable()
	{
		int count;
		count = GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Count;
		WeaponType iD;
		iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
		if (count > 0)
		{
			if (iD == WeaponType.LEVEL_1)
			{
				this.Enable();
			}
			else if (count >= 3 && iD == WeaponType.LEVEL_3)
			{
				this.Enable();
			}
			else if (count >= 2 && iD == WeaponType.LEVEL_2)
			{
				this.Enable();
			}
		}
	}

	private void Enable()
	{
		this.m_MeshOutline.Enable();
		this.SetActive(active: true);
		this.m_Interactable.SetActive(active: true);
		this.m_Interactable.ResetAction();
	}

	private void Disable()
	{
		this.m_MeshOutline.SetActive(active: false);
		this.SetActive(active: false);
		this.m_Interactable.SetActive(active: false);
	}

	private void GetWeapon()
	{
		this.RemoveWeapon();
		this.m_Weapon = global::UnityEngine.Object.Instantiate(GameManager.Instance.Player.ActiveWeaponDuplicate);
		this.m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent);
		this.m_Weapon.transform.localPosition = Vector3.zero;
		this.m_Weapon.transform.localEulerAngles = Vector3.zero;
		this.m_Weapon.transform.localScale = GameManager.Instance.Player.ActiveWeaponDuplicate.transform.localScale;
		MeshRenderer[] componentsInChildren;
		componentsInChildren = this.m_Weapon.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
		}
		this.m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent.parent);
	}

	public void Upgrade()
	{
		WeaponType iD;
		iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
		switch (iD)
		{
		case WeaponType.LEVEL_1:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBatteryCasing(GentUpgradesRequirements.GetBatteryCasings(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			break;
		case WeaponType.LEVEL_2:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(GentUpgradesRequirements.GetBatteries(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
			break;
		case WeaponType.LEVEL_3:
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBatteryCasing(GentUpgradesRequirements.GetBatteryCasings(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBattery(GentUpgradesRequirements.GetBatteries(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentUpgradesRequirements.GetToolkits(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentUpgradesRequirements.GetParts(iD));
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
			break;
		}
		this.m_Particles.SetActive(value: true);
		this.ResetSequence();
		this.m_Sequence.InsertCallback(6f, () =>
		{
			this.m_Particles.SetActive(value: false);
		});
		this.m_Sequence.InsertCallback(5f, () =>
		{
			WeaponType iD2;
			iD2 = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
			WeaponType type;
			type = iD2;
			string key;
			key = "NOTIFICATION_GENT_PIPE_GENT_LOCK";
			switch (iD2)
			{
			case WeaponType.LEVEL_1:
				type = WeaponType.LEVEL_2;
				break;
			case WeaponType.LEVEL_2:
				this.m_WasUpgraded = true;
				type = WeaponType.LEVEL_3;
				key = "NOTIFICATION_GENT_PIPE_SHOCK_PIPE";
				break;
			case WeaponType.LEVEL_3:
				this.m_WasUpgraded = true;
				type = WeaponType.LEVEL_4;
				key = "NOTIFICATION_GENT_PIPE_STUN_PIPE";
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GENTS_FINEST);
				break;
			}
			GameManager.Instance.Player.SetNewWeapon(new WeaponData(type));
			this.GetWeapon();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_GENT_PIPE_UPGRADED") + ": " + TextUtility.GetKey(key));
			GameManager.Instance.ClearGentUpgrades();
		});
		this.OnUpgrade.Send(this);
	}

	public void Close()
	{
		this.m_Interactable.OnInteractionExit -= this.HandleInteractableOnInteractionExit;
		this.m_Interactable.OnInteractionExit += this.HandleInteractableOnInteractionExit;
		this.m_Interactable.Exit();
	}

	private void HandleInteractableOnInteractionExit(object sender, EventArgs e)
	{
		this.m_Interactable.OnInteractionExit -= this.HandleInteractableOnInteractionExit;
		GameManager.Instance.Player.SetCollision(active: true);
		this.RemoveWeapon();
		this.CheckEnable();
		if (this.m_WasUpgraded)
		{
			string input;
			input = "TAB";
			if (GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == WeaponType.LEVEL_3)
			{
				GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(InfoPopupType.ShockPipe, input));
			}
			else if (GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == WeaponType.LEVEL_4)
			{
				GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(InfoPopupType.StunPipe, input));
			}
		}
		this.m_WasUpgraded = false;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: false);
		UIManager.SetCursor(active: false);
		this.GetWeapon();
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowGentUpgrades(this);
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
		float value;
		value = (this.IsActive ? 1f : 0f);
		this.m_EmissionRenderer.material.SetFloat("_Power", value);
		this.m_Lighting.SetActive(this.IsActive);
	}

	private void RemoveWeapon()
	{
		if (this.m_Weapon != null)
		{
			global::UnityEngine.Object.Destroy(this.m_Weapon);
			this.m_Weapon = null;
		}
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

	private void RemoveListeners()
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteractionComplete -= this.HandleInteractableOnInteractionComplete;
		this.m_Interactable.OnInteractionExit -= this.HandleInteractableOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.OnUpgrade = null;
		this.m_Weapon = null;
		this.KillSequence();
		this.RemoveListeners();
		base.OnDisposed();
	}
}
