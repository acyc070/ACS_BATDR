using System;
using UnityEngine;

[Serializable]
public class PlayerDataDirectory
{
	[SerializeField]
	private PlayerTransform m_Transform = new PlayerTransform();

	[SerializeField]
	private PlayerStatistics m_Statistics = new PlayerStatistics();

	[SerializeField]
	private WeaponDataObject m_WeaponData = new WeaponDataObject();

	[SerializeField]
	private AbilityDataDirectory m_AbilityDirectory = new AbilityDataDirectory();

	public PlayerTransform Transform => this.m_Transform;

	public PlayerStatistics Statistics => this.m_Statistics;

	public WeaponDataObject WeaponData => this.m_WeaponData;

	public AbilityDataDirectory AbilityDirectory => this.m_AbilityDirectory;
}
