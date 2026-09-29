using System;
using UnityEngine;

public class ObjectiveWeapon : Objective
{
	[Serializable]
	public enum WeaponCheckType
	{
		Has = 0,
		Get = 3,
		Remove = 100
	}

	[SerializeField]
	private WeaponType m_WeaponType;

	[SerializeField]
	private WeaponCheckType m_CheckType;

	[SerializeField]
	private GameObject m_Target;

	[SerializeField]
	private WeaponData m_WeaponData;

	private bool m_IsChecking;

	protected override void InternalInitialize()
	{
		if (this.m_CheckType == WeaponCheckType.Has)
		{
			this.m_IsChecking = true;
		}
		else if (this.m_CheckType == WeaponCheckType.Get)
		{
			GameManager.Instance.Player.EquipWeapon(this.m_WeaponData);
			GameManager.Instance.ShowCrosshair();
			base.SendOnComplete();
		}
		else if (this.m_CheckType == WeaponCheckType.Remove)
		{
			GameManager.Instance.Player.RemoveWeapon();
			GameManager.Instance.ShowCrosshair();
			base.SendOnComplete();
		}
	}

	protected override void InternalComplete()
	{
		this.Cancel();
	}

	protected override void InternalForceComplete()
	{
		this.Cancel();
	}

	private void Update()
	{
		if (this.m_IsChecking && !base.IsDisposed && !base.IsComplete && !GameManager.Instance.IsPaused && this.m_CheckType == WeaponCheckType.Has && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.Weapon.Data.WeaponType == this.m_WeaponType)
		{
			base.SendOnComplete();
		}
	}

	private void Cancel()
	{
		this.m_IsChecking = false;
	}
}
