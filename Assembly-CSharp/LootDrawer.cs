using UnityEngine;

public class LootDrawer : JMonoBehaviour
{
	[SerializeField]
	private LootDrawerType m_LootDrawerType;

	public LootDrawerType LootDrawerType => this.m_LootDrawerType;
}
