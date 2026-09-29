public class UIElementTitleMenuButton : UIElementButtonLabel
{
	private TitleMenuType m_TitleMenuType;

	public TitleMenuType TitleMenuType => this.m_TitleMenuType;

	public void SetType(TitleMenuType titleMenuType)
	{
		this.m_TitleMenuType = titleMenuType;
	}
}
