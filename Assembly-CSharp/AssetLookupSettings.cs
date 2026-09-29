using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Project/AssetLookupSettings/Create")]
public class AssetLookupSettings : ScriptableObject
{
	[SerializeField]
	public string Namespace = "UnityEngine";

	[SerializeField]
	public string StringLookupClass = "AssetLookup";

	[SerializeField]
	public string ResourcesFilePath = "Assets/JoeyDrewStudios/Tools/AssetLookup/Editor/Resources/";

	[SerializeField]
	public string ScriptFilePath = "Assets/JoeyDrewStudios/Tools/AssetLookup/Runtime/Scripts/";

	[SerializeField]
	public string LookupFilePath = "Lookup/";

	[SerializeField]
	public bool HasWhiteList;

	[SerializeField]
	public List<string> WhiteList = new List<string>();

	protected static AssetLookupSettings m_Instance;

	public static AssetLookupSettings Instance => AssetLookupSettings.m_Instance ?? (AssetLookupSettings.m_Instance = Resources.Load<AssetLookupSettings>("AssetLookupSettings"));
}
