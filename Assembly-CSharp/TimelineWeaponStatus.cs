using UnityEngine;

public class TimelineWeaponStatus : JMonoBehaviour
{
	[SerializeField]
	private WeaponStatus m_WeaponStatus;

	public void Action()
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
	}
}
