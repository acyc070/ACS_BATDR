using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Present/New Present")]
public class PresentData : ScriptableObject
{
	[SerializeField]
	private string m_PresentName = "";

	public string PresentName => this.m_PresentName;
}
