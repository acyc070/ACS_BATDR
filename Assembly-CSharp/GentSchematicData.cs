using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Gent Schematics/New Gent Schematic")]
public class GentSchematicData : ScriptableObject
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Gent Schematic Identifier")]
	[SerializeField]
	private GentSchematicID m_ID;

	public SectionID SectionID => this.m_SectionID;

	public GentSchematicID ID => this.m_ID;
}
