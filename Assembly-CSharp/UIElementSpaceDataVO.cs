public class UIElementSpaceDataVO : UIElementDataVO
{
	public float Space;

	public UIElementSpaceDataVO(string prefabKey, float space)
		: base(prefabKey)
	{
		this.Space = space;
	}
}
