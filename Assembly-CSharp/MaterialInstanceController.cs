using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialInstanceController : JMonoBehaviour
{
	[SerializeField]
	public enum PropertyContainerType
	{
		TextureProp = 0,
		FloatProp = 1,
		ColorProp = 2,
		BoolProp = 3,
		SliderProp = 4
	}

	[SerializeField]
	private string m_RefrenceShaderName;

	[SerializeField]
	private List<int> m_MaterialHashIDs = new List<int>();

	[SerializeField]
	private List<MaterialInstancePropertyContainer> m_MaterialChangedProperties = new List<MaterialInstancePropertyContainer>();

	private MaterialPropertyBlock m_MaterialProperties;

	private Renderer m_Renderer;

	public Renderer Renderer
	{
		get
		{
			if (this.m_Renderer == null)
			{
				this.m_Renderer = base.GetComponent<Renderer>();
			}
			return this.m_Renderer;
		}
	}

	[ExecuteInEditMode]
	public override void OnEnable()
	{
		base.OnEnable();
		this.ApplyMaterialBlock();
	}

	public string GetShaderType()
	{
		return this.Renderer.sharedMaterial.shader.name;
	}

	private string SetupContainers(string _checkedShader)
	{
		if (this.m_RefrenceShaderName == _checkedShader)
		{
			return _checkedShader;
		}
		return this.m_RefrenceShaderName = _checkedShader;
	}

	public bool CheckPropertyBlockStatus(string _Identifier = "")
	{
		int hashCode;
		hashCode = _Identifier.GetHashCode();
		return this.m_MaterialHashIDs.Contains(hashCode);
	}

	public MaterialInstancePropertyContainer GetPropertyBlock(string _Identifier)
	{
		int hashCode;
		hashCode = _Identifier.GetHashCode();
		return this.m_MaterialChangedProperties[this.m_MaterialHashIDs.IndexOf(hashCode)];
	}

	public void AddPropertyBlock(string _Identifier, PropertyContainerType _propertyType)
	{
		int hashCode;
		hashCode = _Identifier.GetHashCode();
		if (!this.m_MaterialHashIDs.Contains(hashCode))
		{
			if (!this.m_MaterialChangedProperties.Contains(new MaterialInstancePropertyContainer(_propertyType, _Identifier)))
			{
				this.m_MaterialChangedProperties.Add(new MaterialInstancePropertyContainer(_propertyType, _Identifier));
			}
			this.m_MaterialHashIDs.Add(hashCode);
		}
	}

	public void RemovePropertyBlock(string _Identifier)
	{
		int hashCode;
		hashCode = _Identifier.GetHashCode();
		if (this.m_MaterialHashIDs.Contains(hashCode))
		{
			this.m_MaterialChangedProperties.RemoveAt(this.m_MaterialHashIDs.IndexOf(hashCode));
			this.m_MaterialHashIDs.Remove(hashCode);
		}
	}

	public void ApplyMaterialBlock()
	{
		this.m_MaterialProperties = new MaterialPropertyBlock();
		for (int i = 0; i < this.m_MaterialChangedProperties.Count; i = unchecked(i + 1))
		{
			MaterialInstancePropertyContainer materialInstancePropertyContainer;
			materialInstancePropertyContainer = this.m_MaterialChangedProperties[i];
			switch (materialInstancePropertyContainer.PropertyType)
			{
			case PropertyContainerType.TextureProp:
				if (materialInstancePropertyContainer.NewTexture != null)
				{
					this.m_MaterialProperties.SetTexture(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewTexture);
				}
				break;
			case PropertyContainerType.FloatProp:
			case PropertyContainerType.BoolProp:
			case PropertyContainerType.SliderProp:
				this.m_MaterialProperties.SetFloat(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewFloat);
				break;
			case PropertyContainerType.ColorProp:
				this.m_MaterialProperties.SetColor(materialInstancePropertyContainer.Name, materialInstancePropertyContainer.NewColor);
				break;
			}
		}
		this.Renderer.SetPropertyBlock(this.m_MaterialProperties);
	}

	public void ClearMaterialBlock()
	{
		this.Renderer.SetPropertyBlock(null);
	}
}
