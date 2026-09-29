using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Audio Selector")]
public class CharacterAudioSelector : ScriptableObject
{
	[SerializeField]
	private GameObject[] m_Prefabs;

	private List<int> m_History = new List<int>();

	private int m_SelectedIndex = -1;

	public GameObject Get()
	{
		int num;
		num = Random.Range(0, this.m_Prefabs.Length);
		if (this.m_Prefabs.Length > 1)
		{
			if (this.m_History.Count >= this.m_Prefabs.Length)
			{
				this.m_History.Clear();
			}
			while (num == this.m_SelectedIndex && this.m_History.Contains(num))
			{
				num = Random.Range(0, this.m_Prefabs.Length);
			}
		}
		this.m_SelectedIndex = num;
		if (!this.m_History.Contains(this.m_SelectedIndex))
		{
			this.m_History.Add(this.m_SelectedIndex);
		}
		return Object.Instantiate(this.m_Prefabs[this.m_SelectedIndex]);
	}
}
