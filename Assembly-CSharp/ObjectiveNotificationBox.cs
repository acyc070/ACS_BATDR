using UnityEngine;

public class ObjectiveNotificationBox : Objective
{
	[SerializeField]
	private string m_Label = "Collected a Key";

	[SerializeField]
	private bool m_IsKey = true;

	protected override void InternalInitialize()
	{
		if (this.m_IsKey)
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey(this.m_Label), "Icon/Collectables/Small/UIIcon_Key", 1);
		}
		else
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey(this.m_Label));
		}
		base.SendOnComplete();
	}
}
