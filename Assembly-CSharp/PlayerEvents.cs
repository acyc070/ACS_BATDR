using S13Audio;
using UnityEngine;

public class PlayerEvents : JMonoBehaviour
{
	[SerializeField]
	private S13ScriptableEvent m_AbilitiesReturn;

	[SerializeField]
	private S13ScriptableEvent m_AbilitiesLose;

	public void AbilitiesReturn()
	{
		this.m_AbilitiesReturn?.Raise();
	}

	public void AbilitiesLose()
	{
		this.m_AbilitiesLose?.Raise();
	}
}
