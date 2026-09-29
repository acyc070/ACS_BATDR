using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Abilities/New Ability Data")]
public class AbilityData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Banish;

	[SerializeField]
	private Sprite m_Flow;

	[SerializeField]
	private Sprite m_FastTravel;

	public Sprite Banish => this.m_Banish;

	public Sprite Flow => this.m_Flow;

	public Sprite FastTravel => this.m_FastTravel;
}
