using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/InfoPopup/New InfoPopup")]
public class InfoPopupData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Image;

	public Sprite Image => this.m_Image;
}
