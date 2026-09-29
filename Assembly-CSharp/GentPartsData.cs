using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Gent Parts/New Gent Parts")]
public class GentPartsData : ScriptableObject
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private int m_Value = 1;

	public Interactable Interactable => this.m_Interactable;

	public int Value => this.m_Value;
}
