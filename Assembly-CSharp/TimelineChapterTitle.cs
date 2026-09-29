using UnityEngine;

public class TimelineChapterTitle : JMonoBehaviour
{
	[SerializeField]
	private string m_Chapter;

	[SerializeField]
	private string m_Title;

	public void Action()
	{
		this.m_Chapter += ":";
		GameManager.Instance.ShowChapterTitle(this.m_Chapter, this.m_Title);
	}
}
