using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Food/New Food")]
public class FoodData : ScriptableObject
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private string m_FoodName = "";

	[SerializeField]
	private int m_Value;

	public Interactable Interactable => this.m_Interactable;

	public string FoodName => this.m_FoodName;

	public int Value => this.m_Value;
}
