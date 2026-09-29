public class SpriteLookupDataVO : JDisposable
{
	public string Lookup;

	public string Sprite;

	public static SpriteLookupDataVO Create(string lookup, string sprite)
	{
		return new SpriteLookupDataVO(lookup, sprite);
	}

	public SpriteLookupDataVO(string lookup, string sprite)
	{
		this.Lookup = lookup;
		this.Sprite = sprite;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
