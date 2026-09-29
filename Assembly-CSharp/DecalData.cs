using System;
using UnityEngine;

[Serializable]
public class DecalData
{
	public DecalFolderType FolderType;

	public DecalSubFolderType SubFolderType;

	public Material SelectedDecalMaterial;

	public Texture2D Texture;

	public Color Color = Color.white;

	public Texture2D Emissive;

	public Color EmissiveColor = Color.black;

	public Texture2D Normals;

	public float Specular;

	public float Alpha = 1f;
}
