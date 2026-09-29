using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Project/ProjectSettings/Create")]
public class ProjectSettings : ScriptableObject
{
	public int TargetFrameRate = 60;

	public JDebug.JDebugType EnableLogs = JDebug.JDebugType.All;

	public bool VisualizeColliders;

	protected static ProjectSettings m_Instance;

	public static ProjectSettings Instance => ProjectSettings.m_Instance ?? (ProjectSettings.m_Instance = Resources.Load<ProjectSettings>("ProjectSettings"));
}
