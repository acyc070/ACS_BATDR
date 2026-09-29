using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Audio Log/New Audio Log")]
public class AudioLogData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("AudioLog Identifier")]
	[SerializeField]
	private AudioLogID m_ID;

	[Header("AudioLog Speaker Name")]
	[SerializeField]
	private CharacterID m_CharacterName;

	[Header("AudioLog Type")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Audio Clip")]
	[SerializeField]
	private AudioClip m_AudioClip;

	public SectionID SectionID => this.m_SectionID;

	public AudioLogID ID => this.m_ID;

	public CharacterID CharacterID => this.m_CharacterName;

	public Interactable Interactable => this.m_Interactable;

	public AudioClip AudioClip => this.m_AudioClip;
}
