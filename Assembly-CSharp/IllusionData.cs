using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Illusion of Living/New Illusion Book")]
public class IllusionData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Illusion Identifier")]
	[SerializeField]
	private IllusionID m_ID;

	public SectionID SectionID => this.m_SectionID;

	public IllusionID ID => this.m_ID;
}
