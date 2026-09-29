using System;
using UnityEngine;

[DefaultExecutionOrder(1050)]
public class TimelineWeaponChanger : JMonoBehaviour
{
	[Serializable]
	public class WeaponSelector
	{
		public GameObject Weapon;

		public MeshOutline WeaponOutline;
	}

	[SerializeField]
	private WeaponSelector m_WeaponLv2;

	[SerializeField]
	private WeaponSelector m_WeaponLv4;

	public override void Start()
	{
		switch (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID)
		{
		case WeaponType.LEVEL_2:
		case WeaponType.LEVEL_3:
			if (this.m_WeaponLv4 != null)
			{
				if (this.m_WeaponLv4.Weapon != null)
				{
					global::UnityEngine.Object.Destroy(this.m_WeaponLv4.Weapon);
				}
				if (this.m_WeaponLv4.WeaponOutline != null)
				{
					this.m_WeaponLv4.WeaponOutline.Dispose();
				}
				this.m_WeaponLv4 = null;
			}
			break;
		case WeaponType.LEVEL_4:
			if (this.m_WeaponLv2 != null)
			{
				if (this.m_WeaponLv2.Weapon != null)
				{
					global::UnityEngine.Object.Destroy(this.m_WeaponLv2.Weapon);
				}
				if (this.m_WeaponLv2.WeaponOutline != null)
				{
					this.m_WeaponLv2.WeaponOutline.Dispose();
				}
				this.m_WeaponLv2 = null;
			}
			break;
		}
	}
}
