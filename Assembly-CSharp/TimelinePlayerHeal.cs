using UnityEngine;

public class TimelinePlayerHeal : JMonoBehaviour
{
	[SerializeField]
	private int m_Amount;

	public void Action()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Heal(this.m_Amount);
		}
	}
}
