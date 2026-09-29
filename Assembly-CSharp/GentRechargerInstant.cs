using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GentRechargerInstant : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationSingle m_Interactable;

	[SerializeField]
	private ParticleSystem[] m_Particles;

	[SerializeField]
	private GameObject m_ActiveContent;

	[SerializeField]
	private bool m_ActiveOnStart;

	private GameObject m_Weapon;

	private Sequence m_Sequence;

	private bool m_IsActive;

	public override void Start()
	{
		this.RemoveListeners();
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteractionComplete += this.HandleInteractableOnInteractionComplete;
		this.SetActive(this.m_ActiveOnStart);
		this.m_Interactable.SetActive(active: false);
	}

	private void Update()
	{
		if (!this.m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType != WeaponType.LEVEL_1)
		{
			int power;
			power = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power;
			if (!this.m_Interactable.IsActive)
			{
				if (power < 6)
				{
					this.m_Interactable.SetActive(active: true);
				}
			}
			else if (power >= 6)
			{
				this.m_Interactable.SetActive(active: false);
			}
		}
		else if (this.m_Interactable.IsActive)
		{
			this.m_Interactable.SetActive(active: false);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		unchecked
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
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].shadowCastingMode = ShadowCastingMode.On;
				}
				this.m_Weapon.transform.SetParent(GameManager.Instance.Player.BodyWeaponParent.parent);
			}
			GentPipePowerIndicator[] componentsInChildren2;
			componentsInChildren2 = GameManager.Instance.Player.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
			}
			this.ResetSequence();
			this.m_Sequence.InsertCallback(0.9f, this.SetPowerIndicator);
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
		this.m_ActiveContent.SetActive(this.m_IsActive);
	}

	private void HandleInteractableOnInteractionComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetCollision(active: true);
		if (this.m_Weapon != null)
		{
			global::UnityEngine.Object.Destroy(this.m_Weapon);
			this.m_Weapon = null;
		}
	}

	private void SetPowerIndicator()
	{
		unchecked
		{
			if (this.m_Particles != null)
			{
				for (int i = 0; i < this.m_Particles.Length; i++)
				{
					this.m_Particles[i].Play();
				}
			}
			GentPipePowerIndicator[] componentsInChildren;
			componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SetPowerLevel(6);
			}
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(6);
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
	}

	protected override void OnDisposed()
	{
		this.m_Weapon = null;
		this.KillSequence();
		this.RemoveListeners();
		base.OnDisposed();
	}
}
