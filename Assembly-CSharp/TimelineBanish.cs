using UnityEngine;

public class TimelineBanish : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_GO;

	[SerializeField]
	private float m_Duration = 5.25f;

	public void Action()
	{
		Banish.DOBanish(this.m_GO, this.m_Duration);
	}
}
