public class ArchGateIntro : JMonoBehaviour
{
	public void Action()
	{
		GameManager.Instance.UIManager.Show<UIArchGateIntro>("UI/Views/UIArchGateIntro", "UPPER BLOCKER");
	}
}
