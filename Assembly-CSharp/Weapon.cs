using System;
using UnityEngine;

public class Weapon : Item<WeaponContent, WeaponData>
{
	public event EventHandler OnEquip;

	public event EventHandler OnUnequip;

	public event EventHandler OnAttack;

	public event EventHandler OnAttackComplete;

	public override void Initialize(object data)
	{
		if (data == null)
		{
			base.Dispose();
			return;
		}
		base.Initialize(data);
		base.m_Content = base.GetComponentInChildren<WeaponContent>();
		string assetKey;
		assetKey = "";
		if (base.Data.WeaponType == WeaponType.LEVEL_1)
		{
			assetKey = "Weapons/Weapon_GentPipe_01";
		}
		else if (base.Data.WeaponType == WeaponType.LEVEL_2)
		{
			assetKey = "Weapons/Weapon_GentPipe_02";
		}
		else if (base.Data.WeaponType == WeaponType.LEVEL_3)
		{
			assetKey = "Weapons/Weapon_GentPipe_03";
		}
		else if (base.Data.WeaponType == WeaponType.LEVEL_4)
		{
			assetKey = "Weapons/Weapon_GentPipe_04";
		}
		Transform transform;
		transform = GameManager.Instance.AssetManager.CreateAsset<Transform>(assetKey);
		transform.SetParent(base.Content.transform);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		base.Content.Initialize(this);
	}

	public void SendOnEquip()
	{
		this.OnEquip.Send(this);
	}

	public void SendOnUnequip()
	{
		this.OnUnequip.Send(this);
	}

	public void SendOnAttack()
	{
		this.OnAttack.Send(this);
	}

	public void SendOnAttackComplete()
	{
		this.OnAttackComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnEquip = null;
		this.OnUnequip = null;
		this.OnAttack = null;
		this.OnAttackComplete = null;
		base.OnDisposed();
	}
}
