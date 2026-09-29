public class JDSPresents : JMonoBehaviour
{
	public void Action()
	{
		GameManager.Instance.UIManager.Show<UIArchGateJDSPresents>("UI/Views/UIArchGateJDSPresents", "VIEW", new UILabelDataVO("PRESENTS"));
	}
}
