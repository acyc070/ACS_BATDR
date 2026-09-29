using UnityEngine;
using UnityEngine.UI;

public class UIElementHealthStatus : UIElement
{
	[SerializeField]
	private Image m_HealthFiller;

	public override void Initialize(object _data)
	{
		this.m_HealthFiller.fillAmount = GameManager.Instance.Player.Health / UpgradeCheck.GetHealth();
	}
}
