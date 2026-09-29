public class Disable : JComponent
{
	public override void Awake()
	{
		base.gameObject.SetActive(value: false);
	}
}
