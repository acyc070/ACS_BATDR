public class UIMemoDataVO
{
	public Memo Memo;

	public MemoType MemoType;

	public string MemoLabel;

	public UIMemoDataVO(Memo memo, MemoType memoType, string memoLabel = "")
	{
		this.Memo = memo;
		this.MemoType = memoType;
		this.MemoLabel = memoLabel;
	}
}
