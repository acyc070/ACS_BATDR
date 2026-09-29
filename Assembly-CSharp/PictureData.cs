using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Pictures/New Picture")]
public class PictureData : ScriptableObject
{
	[SerializeField]
	private Interactable m_Interactable;

	public Interactable Interactable => this.m_Interactable;
}
