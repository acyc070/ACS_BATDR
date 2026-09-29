using UnityEngine;

[CreateAssetMenu(menuName = "Character/Enemy Selector")]
public class EnemySelector : ScriptableObject
{
	[SerializeField]
	private Enemy m_Prefab;

	[SerializeField]
	private CharacterContent[] m_Prefabs;

	private int m_SelectedIndex = -1;

	public Enemy Get()
	{
		Enemy enemy;
		enemy = Object.Instantiate(this.m_Prefab);
		CharacterContent content;
		content = this.GetContent();
		content.transform.SetParent(enemy.transform);
		content.transform.localPosition = Vector3.zero;
		content.transform.localEulerAngles = Vector3.zero;
		content.transform.localScale = Vector3.one;
		return enemy;
	}

	private CharacterContent GetContent()
	{
		int num;
		num = Random.Range(0, this.m_Prefabs.Length);
		if (this.m_Prefabs.Length > 1)
		{
			while (num == this.m_SelectedIndex)
			{
				num = Random.Range(0, this.m_Prefabs.Length);
			}
		}
		this.m_SelectedIndex = num;
		return Object.Instantiate(this.m_Prefabs[this.m_SelectedIndex]);
	}
}
