using System;
using UnityEngine;

[Serializable]
public class WeaponDataObject : DataObject<WeaponType, WeaponDataObject>, IDataObject<WeaponType>, IDataObject
{
	[SerializeField]
	private WeaponType m_WeaponID;

	[SerializeField]
	private WeaponStatus m_WeaponStatus;

	[SerializeField]
	private int m_Power;

	public override WeaponType ID => this.m_WeaponID;

	public WeaponStatus WeaponStatus => this.m_WeaponStatus;

	public int Power => this.m_Power;

	protected override void Deserialize()
	{
		this.m_WeaponID = base.m_ID;
	}

	public void SetWeapon(WeaponType id)
	{
		this.m_WeaponID = id;
	}

	public void SetStatus(WeaponStatus status)
	{
		this.m_WeaponStatus = status;
	}

	public void SetPower(int power)
	{
		this.m_Power = power;
	}
}
