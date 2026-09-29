using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Books/New Book")]
public class BookData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Book Identifier")]
	[SerializeField]
	private BookID m_ID;

	[Header("Book Identifier")]
	[SerializeField]
	private BookType m_BookType;

	public SectionID SectionID => this.m_SectionID;

	public BookID ID => this.m_ID;

	public BookType BookType => this.m_BookType;
}
