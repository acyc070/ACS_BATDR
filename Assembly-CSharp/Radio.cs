public class Radio : JMonoBehaviour
{
	public RadioContent Content { get; private set; }

	public override void Awake()
	{
		this.Content = base.gameObject.GetComponentInChildren<RadioContent>();
	}
}
