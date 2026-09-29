using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Project/DecalSettings/Create")]
public class DecalSettings : ScriptableObject
{
	public string FolderName = "Assets/_Game/GameResources/Resources/Decals/";

	public bool ShowIcon = true;

	public bool AllowIconScaling = true;

	public bool ShowGizmos = true;

	public bool ShowAffectedWireframes = true;

	public bool ShowDecalWireframe = true;

	public bool ShowSelectionCube = true;

	public bool AlignToView;

	public float DefaultScale = 5f;

	public float MaxAngle = 90f;

	public float PushDistance = 0.001f;

	public LayerMask AffectedLayers = -2;

	protected static DecalSettings m_Instance;

	public string DEFAULT_FOLDER_NAME => "Assets/_Game/GameResources/Resources/Decals/";

	public string DEFAULT_ICON => "Assets/JoeyDrewStudios/Tools/Decals/Runtime/Gizmos/DecalGizmo.png";

	public static DecalSettings Instance => DecalSettings.m_Instance ?? (DecalSettings.m_Instance = Resources.Load<DecalSettings>("DecalSettings"));
}
