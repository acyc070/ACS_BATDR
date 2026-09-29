using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftMasking;

internal class MaterialReplacements
{
	private class MaterialOverride
	{
		private int _useCount;

		public Material original { get; private set; }

		public Material replacement { get; private set; }

		public MaterialOverride(Material original, Material replacement)
		{
			this.original = original;
			this.replacement = replacement;
			this._useCount = 1;
		}

		public Material Get()
		{
			unchecked
			{
				this._useCount++;
				return this.replacement;
			}
		}

		public bool Release()
		{
			return unchecked(--this._useCount) == 0;
		}
	}

	private readonly IMaterialReplacer _replacer;

	private readonly Action<Material> _applyParameters;

	private readonly List<MaterialOverride> _overrides = new List<MaterialOverride>();

	public MaterialReplacements(IMaterialReplacer replacer, Action<Material> applyParameters)
	{
		this._replacer = replacer;
		this._applyParameters = applyParameters;
	}

	public Material Get(Material original)
	{
		for (int i = 0; i < this._overrides.Count; i = unchecked(i + 1))
		{
			MaterialOverride materialOverride;
			materialOverride = this._overrides[i];
			if ((object)materialOverride.original == original)
			{
				Material material;
				material = materialOverride.Get();
				if ((bool)material)
				{
					material.CopyPropertiesFromMaterial(original);
					this._applyParameters(material);
				}
				return material;
			}
		}
		Material material2;
		material2 = this._replacer.Replace(original);
		if ((bool)material2)
		{
			material2.hideFlags = HideFlags.HideAndDontSave;
			this._applyParameters(material2);
		}
		this._overrides.Add(new MaterialOverride(original, material2));
		return material2;
	}

	public void Release(Material replacement)
	{
		for (int i = 0; i < this._overrides.Count; i = unchecked(i + 1))
		{
			MaterialOverride materialOverride;
			materialOverride = this._overrides[i];
			if (materialOverride.replacement == replacement && materialOverride.Release())
			{
				global::UnityEngine.Object.DestroyImmediate(replacement);
				this._overrides.RemoveAt(i);
				break;
			}
		}
	}

	public void ApplyAll()
	{
		for (int i = 0; i < this._overrides.Count; i = unchecked(i + 1))
		{
			Material replacement;
			replacement = this._overrides[i].replacement;
			if ((bool)replacement)
			{
				this._applyParameters(replacement);
			}
		}
	}

	public void DestroyAllAndClear()
	{
		for (int i = 0; i < this._overrides.Count; i = unchecked(i + 1))
		{
			global::UnityEngine.Object.DestroyImmediate(this._overrides[i].replacement);
		}
		this._overrides.Clear();
	}
}
