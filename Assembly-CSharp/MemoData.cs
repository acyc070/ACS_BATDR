using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Memos/New Memo")]
public class MemoData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Memo Identifier")]
	[SerializeField]
	private MemoID m_ID;

	[Header("Memo Writer Name")]
	[SerializeField]
	private CharacterID m_CharacterName;

	[Header("Memo Type")]
	[SerializeField]
	private MemoType m_MemoType;

	public SectionID SectionID => this.m_SectionID;

	public MemoID ID => this.m_ID;

	public CharacterID CharacterID => this.m_CharacterName;

	public MemoType MemoType => this.m_MemoType;
}
