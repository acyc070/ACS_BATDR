using UnityEngine;

public class ObjectiveInfoPopup : Objective
{
	[Header("Info Popup Type")]
	[SerializeField]
	private InfoPopupType m_InfoPopupType;

	protected override void InternalInitialize()
	{
		string input;
		input = "";
		if (this.m_InfoPopupType == InfoPopupType.GentPipe)
		{
			input = "LMB";
		}
		else if (this.m_InfoPopupType == InfoPopupType.ShockPipe)
		{
			input = "TAB";
		}
		else if (this.m_InfoPopupType == InfoPopupType.StunPipe)
		{
			input = "TAB";
		}
		GameManager.Instance.ShowInfoPopup(UIInfoPopupDataVO.Create(this.m_InfoPopupType, input));
		base.SendOnComplete();
	}
}
