public class UIInfoPopupDataVO
{
	public string TitleLabel;

	public string HeaderLabel;

	public string MessageLabel;

	public string Input;

	public string Image;

	public UIInfoPopupDataVO(string titleLabel, string headerLabel, string messageLabel, string input, string image)
	{
		this.TitleLabel = titleLabel;
		this.HeaderLabel = headerLabel;
		this.MessageLabel = messageLabel;
		this.Input = input;
		this.Image = image;
	}

	public static UIInfoPopupDataVO Create(InfoPopupType infoPopupType, string input = "")
	{
		return infoPopupType switch
		{
			InfoPopupType.Food => new UIInfoPopupDataVO("UI_INFO_POPUP_FOOD", "UI_INFO_POPUP_FOOD_HEADER", "UI_INFO_POPUP_FOOD_MESSAGE", input, "InfoPopup/UIInfoPopup_Food"), 
			InfoPopupType.Stealth => new UIInfoPopupDataVO("UI_INFO_POPUP_STEALTH", "UI_INFO_POPUP_STEALTH_HEADER", "UI_INFO_POPUP_STEALTH_MESSAGE", input, "InfoPopup/UIInfoPopup_Stealth"), 
			InfoPopupType.GentPipe => new UIInfoPopupDataVO("UI_INFO_POPUP_GENT_PIPE", "UI_INFO_POPUP_GENT_PIPE_HEADER", "UI_INFO_POPUP_GENT_PIPE_MESSAGE", input, "InfoPopup/UIInfoPopup_GentPipe"), 
			InfoPopupType.Banish => new UIInfoPopupDataVO("UI_INFO_POPUP_BANISH", "UI_INFO_POPUP_BANISH_HEADER", "UI_INFO_POPUP_BANISH_MESSAGE", input, "InfoPopup/UIInfoPopup_Banish"), 
			InfoPopupType.InkDemon => new UIInfoPopupDataVO("UI_INFO_POPUP_INK_DEMON", "UI_INFO_POPUP_INK_DEMON_HEADER", "UI_INFO_POPUP_INK_DEMON_MESSAGE", input, "InfoPopup/UIInfoPopup_InkDemon"), 
			InfoPopupType.Flow => new UIInfoPopupDataVO("UI_INFO_POPUP_FLOW", "UI_INFO_POPUP_FLOW_HEADER", "UI_INFO_POPUP_FLOW_MESSAGE", input, "InfoPopup/UIInfoPopup_Flow"), 
			InfoPopupType.GentLock => new UIInfoPopupDataVO("UI_INFO_POPUP_GENT_LOCK", "UI_INFO_POPUP_GENT_LOCK_HEADER", "UI_INFO_POPUP_GENT_LOCK_MESSAGE", input, "InfoPopup/UIInfoPopup_GentLock"), 
			InfoPopupType.FastTravel => new UIInfoPopupDataVO("UI_INFO_POPUP_FAST_TRAVEL", "UI_INFO_POPUP_FAST_TRAVEL_HEADER", "UI_INFO_POPUP_FAST_TRAVEL_MESSAGE", input, "InfoPopup/UIInfoPopup_FastTravel"), 
			InfoPopupType.Subway => new UIInfoPopupDataVO("UI_INFO_POPUP_SUBWAY", "UI_INFO_POPUP_SUBWAY_HEADER", "UI_INFO_POPUP_SUBWAY_MESSAGE", input, "InfoPopup/UIInfoPopup_Subway"), 
			InfoPopupType.ShockPipe => new UIInfoPopupDataVO("UI_INFO_POPUP_SHOCK_PIPE", "UI_INFO_POPUP_SHOCK_PIPE_HEADER", "UI_INFO_POPUP_SHOCK_PIPE_MESSAGE", input, "InfoPopup/UIInfoPopup_ShockPipe"), 
			InfoPopupType.StunPipe => new UIInfoPopupDataVO("UI_INFO_POPUP_STUN_PIPE", "UI_INFO_POPUP_STUN_PIPE_HEADER", "UI_INFO_POPUP_STUN_PIPE_MESSAGE", input, "InfoPopup/UIInfoPopup_StunPipe"), 
			_ => null, 
		};
	}
}
