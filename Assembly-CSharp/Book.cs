using UnityEngine;

public class Book : ActionEventController<BookContent, BookData>
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Book Identifier")]
	[SerializeField]
	private BookID m_ID;

	public SectionID SectionID => this.m_SectionID;

	public BookID ID => this.m_ID;
}
