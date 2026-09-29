using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Upgrades/New Upgrade")]
public class UpgradeData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Health;

	[SerializeField]
	private Sprite m_Stamina;

	[SerializeField]
	private Sprite m_Toner;

	[SerializeField]
	private Sprite m_Blank;

	public Sprite Health => this.m_Health;

	public Sprite Stamina => this.m_Stamina;

	public Sprite Toner => this.m_Toner;

	public Sprite Blank => this.m_Blank;
}
