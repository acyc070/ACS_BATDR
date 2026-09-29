using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Project/VersionSettings/Create")]
public class VersionSettings : ScriptableObject
{
	public int Major = 1;

	public int Minor;

	public int Patch;

	public int Build;

	protected static VersionSettings m_Instance;

	public static VersionSettings Instance => VersionSettings.m_Instance ?? (VersionSettings.m_Instance = Resources.Load<VersionSettings>("VersionSettings"));

	public string GetVersion()
	{
		return "v" + this.Major + "." + this.Minor + "." + this.Patch + "." + this.Build.ToString("0000");
	}

	public void UpdateBuild()
	{
		unchecked
		{
			this.Build++;
		}
	}
}
