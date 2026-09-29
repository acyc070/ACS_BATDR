public class UIElementMemoDataVO : UIElementDataVO
{
	public MemoID ID;

	public UIElementButtonDataVO Button;

	public UIElementMemoDataVO(string prefabKey, MemoID id, UIElementButtonDataVO button)
		: base(prefabKey)
	{
		this.ID = id;
		this.Button = button;
	}
}
