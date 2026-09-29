using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Skull/New Skull")]
public class SeasonalSkullData : ScriptableObject
{
	[SerializeField]
	private string m_SkullName = "";

	public string SkullName => this.m_SkullName;
}
