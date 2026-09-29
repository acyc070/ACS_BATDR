using UnityEngine;

public class ObjectiveWeaponStatus : Objective
{
	[SerializeField]
	private WeaponStatus m_WeaponStatus;

	protected override void InternalInitialize()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetStatus(this.m_WeaponStatus);
		if (this.m_WeaponStatus == WeaponStatus.Active)
		{
			GameManager.Instance.Player.EquipWeapon(new WeaponData(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID));
		}
		else if (this.m_WeaponStatus == WeaponStatus.Inactive)
		{
			GameManager.Instance.Player.RemoveWeapon();
		}
		base.SendOnComplete();
	}
}
