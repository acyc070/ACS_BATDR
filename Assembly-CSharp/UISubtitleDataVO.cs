public class UISubtitleDataVO : JDisposable
{
	public string Subtitles;

	public float Duration;

	public bool IsTrimmed;

	public static UISubtitleDataVO Create(string subtitles, float duration, bool isTrimmed = false)
	{
		return new UISubtitleDataVO
		{
			Subtitles = subtitles,
			Duration = duration,
			IsTrimmed = isTrimmed
		};
	}
}
