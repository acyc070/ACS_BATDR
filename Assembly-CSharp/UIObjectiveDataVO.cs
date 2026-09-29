public class UIObjectiveDataVO : JDisposable
{
	public string HeaderLabel;

	public string MessageLabel;

	public UIObjectiveDataVO(string header, string message)
	{
		this.HeaderLabel = header;
		this.MessageLabel = message;
	}
}
