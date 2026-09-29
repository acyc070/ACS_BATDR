using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Sprites/New Sprite Data")]
public class SpriteData : ScriptableObject
{
	[SerializeField]
	private Sprite m_Sprite;

	public Sprite Sprite => this.m_Sprite;
}
