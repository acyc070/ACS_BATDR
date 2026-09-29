using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Seasonal/New Seasonal Section Data")]
public class SeasonalSectionData : ScriptableObject
{
	[SerializeField]
	private List<SeasonalData> m_SeasonalDatas = new List<SeasonalData>();

	public List<SeasonalData> SeasonalDatas => this.m_SeasonalDatas;
}
