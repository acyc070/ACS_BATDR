using System.Collections.Generic;
using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("")]
public class SoftMaskable : UIBehaviour, IMaterialModifier
{
	private ISoftMask _mask;

	private Graphic _graphic;

	private Material _replacement;

	private bool _affectedByMask;

	private bool _destroyed;

	private static List<ISoftMask> s_softMasks;

	private static List<Canvas> s_canvases;

	public bool shaderIsNotSupported { get; private set; }

	public bool isMaskingEnabled
	{
		get
		{
			if (this.mask != null && this.mask.isAlive && this.mask.isMaskingEnabled)
			{
				return this._affectedByMask;
			}
			return false;
		}
	}

	public ISoftMask mask
	{
		get
		{
			return this._mask;
		}
		private set
		{
			if (this._mask != value)
			{
				if (this._mask != null)
				{
					this.replacement = null;
				}
				this._mask = ((value != null && value.isAlive) ? value : null);
				this.Invalidate();
			}
		}
	}

	private Graphic graphic
	{
		get
		{
			if (!this._graphic)
			{
				return this._graphic = base.GetComponent<Graphic>();
			}
			return this._graphic;
		}
	}

	private Material replacement
	{
		get
		{
			return this._replacement;
		}
		set
		{
			if (this._replacement != value)
			{
				if (this._replacement != null && this.mask != null)
				{
					this.mask.ReleaseReplacement(this._replacement);
				}
				this._replacement = value;
			}
		}
	}

	public Material GetModifiedMaterial(Material baseMaterial)
	{
		if (this.isMaskingEnabled)
		{
			this.replacement = this.mask.GetReplacement(baseMaterial);
			if ((bool)this.replacement)
			{
				this.shaderIsNotSupported = false;
				return this.replacement;
			}
			if (!baseMaterial.HasDefaultUIShader())
			{
				this.SetShaderNotSupported(baseMaterial);
			}
		}
		else
		{
			this.shaderIsNotSupported = false;
			this.replacement = null;
		}
		return baseMaterial;
	}

	public void Invalidate()
	{
		if ((bool)this.graphic)
		{
			this.graphic.SetMaterialDirty();
		}
	}

	public void MaskMightChanged()
	{
		if (this.FindMaskOrDie())
		{
			this.Invalidate();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.hideFlags = HideFlags.HideInInspector;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.FindMaskOrDie())
		{
			this.RequestChildTransformUpdate();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.mask = null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this._destroyed = true;
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		this.FindMaskOrDie();
	}

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		this.FindMaskOrDie();
	}

	private void OnTransformChildrenChanged()
	{
		this.RequestChildTransformUpdate();
	}

	private void RequestChildTransformUpdate()
	{
		if (this.mask != null)
		{
			this.mask.UpdateTransformChildren(base.transform);
		}
	}

	private bool FindMaskOrDie()
	{
		if (this._destroyed)
		{
			return false;
		}
		this.mask = SoftMaskable.NearestMask(base.transform, out this._affectedByMask) ?? SoftMaskable.NearestMask(base.transform, out this._affectedByMask, enabledOnly: false);
		if (this.mask == null)
		{
			this._destroyed = true;
			Object.DestroyImmediate(this);
			return false;
		}
		return true;
	}

	private static ISoftMask NearestMask(Transform transform, out bool processedByThisMask, bool enabledOnly = true)
	{
		processedByThisMask = true;
		Transform transform2;
		transform2 = transform;
		ISoftMask iSoftMask;
		while (true)
		{
			if (!transform2)
			{
				return null;
			}
			if (transform2 != transform)
			{
				iSoftMask = SoftMaskable.GetISoftMask(transform2, enabledOnly);
				if (iSoftMask != null)
				{
					break;
				}
			}
			if (SoftMaskable.IsOverridingSortingCanvas(transform2))
			{
				processedByThisMask = false;
			}
			transform2 = transform2.parent;
		}
		return iSoftMask;
	}

	private static ISoftMask GetISoftMask(Transform current, bool shouldBeEnabled = true)
	{
		ISoftMask component;
		component = SoftMaskable.GetComponent(current, SoftMaskable.s_softMasks);
		if (component != null && component.isAlive && (!shouldBeEnabled || component.isMaskingEnabled))
		{
			return component;
		}
		return null;
	}

	private static bool IsOverridingSortingCanvas(Transform transform)
	{
		Canvas component;
		component = SoftMaskable.GetComponent(transform, SoftMaskable.s_canvases);
		if ((bool)component && component.overrideSorting)
		{
			return true;
		}
		return false;
	}

	private static T GetComponent<T>(Component component, List<T> cachedList) where T : class
	{
		component.GetComponents(cachedList);
		using (new ClearListAtExit<T>(cachedList))
		{
			return (cachedList.Count > 0) ? cachedList[0] : null;
		}
	}

	private void SetShaderNotSupported(Material material)
	{
		if (!this.shaderIsNotSupported)
		{
			Debug.LogWarningFormat(base.gameObject, "SoftMask will not work on {0} because material {1} doesn't support masking. Add masking support to your material or set Graphic's material to None to use a default one.", this.graphic, material);
			this.shaderIsNotSupported = true;
		}
	}

	static SoftMaskable()
	{
		SoftMaskable.s_softMasks = new List<ISoftMask>();
		SoftMaskable.s_canvases = new List<Canvas>();
	}
}
