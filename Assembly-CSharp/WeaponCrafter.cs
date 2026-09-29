using UnityEngine;

public class WeaponCrafter : InteractableInputDisplay
{
	[Header("Weapon Options")]
	[SerializeField]
	private WeaponData m_WeaponData;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		GameManager.Instance.Player.EquipWeapon(this.m_WeaponData);
		GameManager.Instance.ShowCrosshair();
	}
}
