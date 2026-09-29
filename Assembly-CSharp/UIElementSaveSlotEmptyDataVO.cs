public class UIElementSaveSlotEmptyDataVO : UIElementDataVO
{
	public string SaveSlotLabel;

	public string NewGameSaveLabel;

	public UIElementSaveSlotEmptyDataVO(string prefabKey, string saveSlotLabel, string newGameSaveLabel)
		: base(prefabKey)
	{
		this.SaveSlotLabel = saveSlotLabel;
		this.NewGameSaveLabel = newGameSaveLabel;
	}
}
