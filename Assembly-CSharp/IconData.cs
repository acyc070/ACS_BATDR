using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Icons/New Icon")]
public class IconData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Icon;

	public Sprite Icon => this.m_Icon;
}
