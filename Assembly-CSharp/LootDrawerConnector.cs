using UnityEngine;

public class LootDrawerConnector : JMonoBehaviour
{
	[SerializeField]
	private Transform m_ContentConnector;

	[SerializeField]
	private Transform m_Content;

	public void Initialize()
	{
		if (this.m_Content != null && this.m_ContentConnector != null)
		{
			this.m_Content.SetParent(this.m_ContentConnector);
		}
	}
}
