using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Weapon Upgrade/New Weapon Upgrade")]
public class WeaponUpgradeData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Lv2;

	[SerializeField]
	private Sprite m_Lv3;

	[SerializeField]
	private Sprite m_Lv4;

	public Sprite Lv2 => this.m_Lv2;

	public Sprite Lv3 => this.m_Lv3;

	public Sprite Lv4 => this.m_Lv4;
}
