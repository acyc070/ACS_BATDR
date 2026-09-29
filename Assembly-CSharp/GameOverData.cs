using System;
using S13Audio;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Game Over/New Game Over Data")]
public class GameOverData : ScriptableObject
{
	[Serializable]
	public class GameOverGroup
	{
		public string Name;

		public GameOverType GameOverType;

		public Sprite Background;

		public S13Object Audio;
	}

	[SerializeField]
	private GameOverGroup[] m_Group;

	public GameOverGroup[] Group => this.m_Group;
}
