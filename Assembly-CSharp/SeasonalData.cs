using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Seasonal/New Seasonal Data")]
public class SeasonalData : ScriptableObject
{
	[SerializeField]
	private SeasonalType m_SeasonalType;

	[SerializeField]
	private GameObject m_SeasonalObject;

	public SeasonalType SeasonalType => this.m_SeasonalType;

	public GameObject SeasonalObject => this.m_SeasonalObject;
}
