public class UINavInputDataVO : JDisposable
{
	public string Input { get; private set; }

	public string Description { get; private set; }

	public static UINavInputDataVO Create(string input, string description)
	{
		return new UINavInputDataVO
		{
			Input = input,
			Description = description
		};
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
