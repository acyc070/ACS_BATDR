using System;
using System.Collections.Generic;
using System.Linq;
using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace SoftMasking;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("UI/Soft Mask", 14)]
[RequireComponent(typeof(RectTransform))]
[HelpURL("https://docs.google.com/document/d/1Y5YEfiNkGr5RUYZRnsK24r9qptwUftjms7hAk75LUD0")]
public class SoftMask : UIBehaviour, ISoftMask, ICanvasRaycastFilter
{
	[Serializable]
	public enum MaskSource
	{
		Graphic = 0,
		Sprite = 1,
		Texture = 2
	}

	[Serializable]
	public enum BorderMode
	{
		Simple = 0,
		Sliced = 1,
		Tiled = 2
	}

	[Serializable]
	[Flags]
	public enum Errors
	{
		NoError = 0,
		UnsupportedShaders = 1,
		NestedMasks = 2,
		TightPackedSprite = 4,
		AlphaSplitSprite = 8,
		UnsupportedImageType = 0x10,
		UnreadableTexture = 0x20
	}

	private struct SourceParameters
	{
		public Image image;

		public Sprite sprite;

		public BorderMode spriteBorderMode;

		public Texture2D texture;

		public Rect textureUVRect;
	}

	private class MaterialReplacerImpl : IMaterialReplacer
	{
		private readonly SoftMask _owner;

		public int order => 0;

		public MaterialReplacerImpl(SoftMask owner)
		{
			this._owner = owner;
		}

		public Material Replace(Material original)
		{
			if (original == null || original.HasDefaultUIShader())
			{
				return MaterialReplacerImpl.Replace(original, this._owner._defaultShader);
			}
			if (original.HasDefaultETC1UIShader())
			{
				return MaterialReplacerImpl.Replace(original, this._owner._defaultETC1Shader);
			}
			if (original.SupportsSoftMask())
			{
				return new Material(original);
			}
			return null;
		}

		private static Material Replace(Material original, Shader defaultReplacementShader)
		{
			Material material;
			material = (defaultReplacementShader ? new Material(defaultReplacementShader) : null);
			if ((bool)material && (bool)original)
			{
				material.CopyPropertiesFromMaterial(original);
			}
			return material;
		}
	}

	private static class Mathr
	{
		public static Vector4 ToVector(Rect r)
		{
			return new Vector4(r.xMin, r.yMin, r.xMax, r.yMax);
		}

		public static Vector4 Div(Vector4 v, Vector2 s)
		{
			return new Vector4(v.x / s.x, v.y / s.y, v.z / s.x, v.w / s.y);
		}

		public static Vector2 Div(Vector2 v, Vector2 s)
		{
			return new Vector2(v.x / s.x, v.y / s.y);
		}

		public static Vector4 Mul(Vector4 v, Vector2 s)
		{
			return new Vector4(v.x * s.x, v.y * s.y, v.z * s.x, v.w * s.y);
		}

		public static Vector2 Size(Vector4 r)
		{
			return new Vector2(r.z - r.x, r.w - r.y);
		}

		public static Vector4 Move(Vector4 v, Vector2 o)
		{
			return new Vector4(v.x + o.x, v.y + o.y, v.z + o.x, v.w + o.y);
		}

		public static Vector4 BorderOf(Vector4 outer, Vector4 inner)
		{
			return new Vector4(inner.x - outer.x, inner.y - outer.y, outer.z - inner.z, outer.w - inner.w);
		}

		public static Vector4 ApplyBorder(Vector4 v, Vector4 b)
		{
			return new Vector4(v.x + b.x, v.y + b.y, v.z - b.z, v.w - b.w);
		}

		public static Vector2 Min(Vector4 r)
		{
			return new Vector2(r.x, r.y);
		}

		public static Vector2 Max(Vector4 r)
		{
			return new Vector2(r.z, r.w);
		}

		public static Vector2 Remap(Vector2 c, Vector4 from, Vector4 to)
		{
			Vector2 s;
			s = Mathr.Max(from) - Mathr.Min(from);
			return Vector2.Scale(b: Mathr.Max(to) - Mathr.Min(to), a: Mathr.Div(c - Mathr.Min(from), s)) + Mathr.Min(to);
		}

		public static bool Inside(Vector2 v, Vector4 r)
		{
			if (v.x >= r.x && v.y >= r.y && v.x <= r.z)
			{
				return v.y <= r.w;
			}
			return false;
		}
	}

	private struct MaterialParameters
	{
		private static class Ids
		{
			public static readonly int SoftMask;

			public static readonly int SoftMask_Rect;

			public static readonly int SoftMask_UVRect;

			public static readonly int SoftMask_ChannelWeights;

			public static readonly int SoftMask_WorldToMask;

			public static readonly int SoftMask_BorderRect;

			public static readonly int SoftMask_UVBorderRect;

			public static readonly int SoftMask_TileRepeat;

			public static readonly int SoftMask_InvertMask;

			public static readonly int SoftMask_InvertOutsides;

			static Ids()
			{
				Ids.SoftMask = Shader.PropertyToID("_SoftMask");
				Ids.SoftMask_Rect = Shader.PropertyToID("_SoftMask_Rect");
				Ids.SoftMask_UVRect = Shader.PropertyToID("_SoftMask_UVRect");
				Ids.SoftMask_ChannelWeights = Shader.PropertyToID("_SoftMask_ChannelWeights");
				Ids.SoftMask_WorldToMask = Shader.PropertyToID("_SoftMask_WorldToMask");
				Ids.SoftMask_BorderRect = Shader.PropertyToID("_SoftMask_BorderRect");
				Ids.SoftMask_UVBorderRect = Shader.PropertyToID("_SoftMask_UVBorderRect");
				Ids.SoftMask_TileRepeat = Shader.PropertyToID("_SoftMask_TileRepeat");
				Ids.SoftMask_InvertMask = Shader.PropertyToID("_SoftMask_InvertMask");
				Ids.SoftMask_InvertOutsides = Shader.PropertyToID("_SoftMask_InvertOutsides");
			}
		}

		public Vector4 maskRect;

		public Vector4 maskBorder;

		public Vector4 maskRectUV;

		public Vector4 maskBorderUV;

		public Vector2 tileRepeat;

		public Color maskChannelWeights;

		public Matrix4x4 worldToMask;

		public Texture2D texture;

		public BorderMode borderMode;

		public bool invertMask;

		public bool invertOutsides;

		public Texture2D activeTexture
		{
			get
			{
				if (!this.texture)
				{
					return Texture2D.whiteTexture;
				}
				return this.texture;
			}
		}

		public bool SampleMask(Vector2 localPos, out float mask)
		{
			Vector2 vector;
			vector = this.XY2UV(localPos);
			try
			{
				mask = this.MaskValue(this.texture.GetPixelBilinear(vector.x, vector.y));
				return true;
			}
			catch (UnityException)
			{
				mask = 0f;
				return false;
			}
		}

		public void Apply(Material mat)
		{
			mat.SetTexture(Ids.SoftMask, this.activeTexture);
			mat.SetVector(Ids.SoftMask_Rect, this.maskRect);
			mat.SetVector(Ids.SoftMask_UVRect, this.maskRectUV);
			mat.SetColor(Ids.SoftMask_ChannelWeights, this.maskChannelWeights);
			mat.SetMatrix(Ids.SoftMask_WorldToMask, this.worldToMask);
			mat.SetFloat(Ids.SoftMask_InvertMask, this.invertMask ? 1 : 0);
			mat.SetFloat(Ids.SoftMask_InvertOutsides, this.invertOutsides ? 1 : 0);
			mat.EnableKeyword("SOFTMASK_SIMPLE", this.borderMode == BorderMode.Simple);
			mat.EnableKeyword("SOFTMASK_SLICED", this.borderMode == BorderMode.Sliced);
			mat.EnableKeyword("SOFTMASK_TILED", this.borderMode == BorderMode.Tiled);
			if (this.borderMode != BorderMode.Simple)
			{
				mat.SetVector(Ids.SoftMask_BorderRect, this.maskBorder);
				mat.SetVector(Ids.SoftMask_UVBorderRect, this.maskBorderUV);
				if (this.borderMode == BorderMode.Tiled)
				{
					mat.SetVector(Ids.SoftMask_TileRepeat, this.tileRepeat);
				}
			}
		}

		private Vector2 XY2UV(Vector2 localPos)
		{
			switch (this.borderMode)
			{
			case BorderMode.Simple:
				return this.MapSimple(localPos);
			case BorderMode.Sliced:
				return this.MapBorder(localPos, repeat: false);
			case BorderMode.Tiled:
				return this.MapBorder(localPos, repeat: true);
			default:
				Debug.LogError("Unknown BorderMode");
				return this.MapSimple(localPos);
			}
		}

		private Vector2 MapSimple(Vector2 localPos)
		{
			return Mathr.Remap(localPos, this.maskRect, this.maskRectUV);
		}

		private Vector2 MapBorder(Vector2 localPos, bool repeat)
		{
			return new Vector2(this.Inset(localPos.x, this.maskRect.x, this.maskBorder.x, this.maskBorder.z, this.maskRect.z, this.maskRectUV.x, this.maskBorderUV.x, this.maskBorderUV.z, this.maskRectUV.z, repeat ? this.tileRepeat.x : 1f), this.Inset(localPos.y, this.maskRect.y, this.maskBorder.y, this.maskBorder.w, this.maskRect.w, this.maskRectUV.y, this.maskBorderUV.y, this.maskBorderUV.w, this.maskRectUV.w, repeat ? this.tileRepeat.y : 1f));
		}

		private float Inset(float v, float x1, float x2, float u1, float u2, float repeat = 1f)
		{
			float num;
			num = x2 - x1;
			return Mathf.Lerp(u1, u2, (num != 0f) ? this.Frac((v - x1) / num * repeat) : 0f);
		}

		private float Inset(float v, float x1, float x2, float x3, float x4, float u1, float u2, float u3, float u4, float repeat = 1f)
		{
			if (v < x2)
			{
				return this.Inset(v, x1, x2, u1, u2);
			}
			if (v < x3)
			{
				return this.Inset(v, x2, x3, u2, u3, repeat);
			}
			return this.Inset(v, x3, x4, u3, u4);
		}

		private float Frac(float v)
		{
			return v - Mathf.Floor(v);
		}

		private float MaskValue(Color mask)
		{
			Color color;
			color = mask * this.maskChannelWeights;
			return color.a + color.r + color.g + color.b;
		}
	}

	private struct Diagnostics(SoftMask softMask)
	{
		private SoftMask _softMask = softMask;

		private Image image => this._softMask.DeduceSourceParameters().image;

		private Sprite sprite => this._softMask.DeduceSourceParameters().sprite;

		private Texture2D texture => this._softMask.DeduceSourceParameters().texture;

		public Errors PollErrors()
		{
			SoftMask softMask;
			softMask = this._softMask;
			Errors errors;
			errors = Errors.NoError;
			softMask.GetComponentsInChildren(SoftMask.s_maskables);
			using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
			{
				if (SoftMask.s_maskables.Any((SoftMaskable m) => m.mask == softMask && m.shaderIsNotSupported))
				{
					errors |= Errors.UnsupportedShaders;
				}
			}
			if (this.ThereAreNestedMasks())
			{
				errors |= Errors.NestedMasks;
			}
			return errors | Diagnostics.CheckSprite(this.sprite) | this.CheckImage() | this.CheckTexture();
		}

		public static Errors CheckSprite(Sprite sprite)
		{
			Errors errors;
			errors = Errors.NoError;
			if (!sprite)
			{
				return errors;
			}
			if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
			{
				errors |= Errors.TightPackedSprite;
			}
			if ((bool)sprite.associatedAlphaSplitTexture)
			{
				errors |= Errors.AlphaSplitSprite;
			}
			return errors;
		}

		private bool ThereAreNestedMasks()
		{
			SoftMask softMask;
			softMask = this._softMask;
			bool flag;
			flag = false;
			using (new ClearListAtExit<SoftMask>(SoftMask.s_masks))
			{
				softMask.GetComponentsInParent(includeInactive: false, SoftMask.s_masks);
				flag |= SoftMask.s_masks.Any((SoftMask x) => Diagnostics.AreCompeting(softMask, x));
				softMask.GetComponentsInChildren(includeInactive: false, SoftMask.s_masks);
				return flag | SoftMask.s_masks.Any((SoftMask x) => Diagnostics.AreCompeting(softMask, x));
			}
		}

		private Errors CheckImage()
		{
			Errors errors;
			errors = Errors.NoError;
			if (!this._softMask.isBasedOnGraphic)
			{
				return errors;
			}
			if ((bool)this.image && !Diagnostics.IsSupportedImageType(this.image.type))
			{
				errors |= Errors.UnsupportedImageType;
			}
			return errors;
		}

		private Errors CheckTexture()
		{
			Errors errors;
			errors = Errors.NoError;
			if (this._softMask.isUsingRaycastFiltering && (bool)this.texture && !Diagnostics.IsReadable(this.texture))
			{
				errors |= Errors.UnreadableTexture;
			}
			return errors;
		}

		private static bool AreCompeting(SoftMask softMask, SoftMask other)
		{
			if (softMask.isMaskingEnabled && softMask != other && other.isMaskingEnabled && softMask.canvas.rootCanvas == other.canvas.rootCanvas)
			{
				return !Diagnostics.SelectChild(softMask, other).canvas.overrideSorting;
			}
			return false;
		}

		private static T SelectChild<T>(T first, T second) where T : Component
		{
			if (!first.transform.IsChildOf(second.transform))
			{
				return second;
			}
			return first;
		}

		private static bool IsReadable(Texture2D texture)
		{
			try
			{
				texture.GetPixel(0, 0);
				return true;
			}
			catch (UnityException)
			{
				return false;
			}
		}

		private static bool IsSupportedImageType(Image.Type type)
		{
			if (type != Image.Type.Simple && type != Image.Type.Sliced)
			{
				return type == Image.Type.Tiled;
			}
			return true;
		}
	}

	[SerializeField]
	private Shader _defaultShader;

	[SerializeField]
	private Shader _defaultETC1Shader;

	[SerializeField]
	private MaskSource _source;

	[SerializeField]
	private RectTransform _separateMask;

	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	private BorderMode _spriteBorderMode;

	[SerializeField]
	private Texture2D _texture;

	[SerializeField]
	private Rect _textureUVRect = SoftMask.DefaultUVRect;

	[SerializeField]
	private Color _channelWeights = MaskChannel.alpha;

	[SerializeField]
	private float _raycastThreshold;

	[SerializeField]
	private bool _invertMask;

	[SerializeField]
	private bool _invertOutsides;

	private MaterialReplacements _materials;

	private MaterialParameters _parameters;

	private Sprite _lastUsedSprite;

	private Rect _lastMaskRect;

	private bool _maskingWasEnabled;

	private bool _destroyed;

	private bool _dirty;

	private RectTransform _maskTransform;

	private Graphic _graphic;

	private Canvas _canvas;

	private static readonly Rect DefaultUVRect;

	private static readonly List<SoftMask> s_masks;

	private static readonly List<SoftMaskable> s_maskables;

	public Shader defaultShader
	{
		get
		{
			return this._defaultShader;
		}
		set
		{
			this.SetShader(ref this._defaultShader, value);
		}
	}

	public Shader defaultETC1Shader
	{
		get
		{
			return this._defaultETC1Shader;
		}
		set
		{
			this.SetShader(ref this._defaultETC1Shader, value, warnIfNotSet: false);
		}
	}

	public MaskSource source
	{
		get
		{
			return this._source;
		}
		set
		{
			if (this._source != value)
			{
				this.Set(ref this._source, value);
			}
		}
	}

	public RectTransform separateMask
	{
		get
		{
			return this._separateMask;
		}
		set
		{
			if (this._separateMask != value)
			{
				this.Set(ref this._separateMask, value);
				this._graphic = null;
				this._maskTransform = null;
			}
		}
	}

	public Sprite sprite
	{
		get
		{
			return this._sprite;
		}
		set
		{
			if (this._sprite != value)
			{
				this.Set(ref this._sprite, value);
			}
		}
	}

	public BorderMode spriteBorderMode
	{
		get
		{
			return this._spriteBorderMode;
		}
		set
		{
			if (this._spriteBorderMode != value)
			{
				this.Set(ref this._spriteBorderMode, value);
			}
		}
	}

	public Texture2D texture
	{
		get
		{
			return this._texture;
		}
		set
		{
			if (this._texture != value)
			{
				this.Set(ref this._texture, value);
			}
		}
	}

	public Rect textureUVRect
	{
		get
		{
			return this._textureUVRect;
		}
		set
		{
			if (this._textureUVRect != value)
			{
				this.Set(ref this._textureUVRect, value);
			}
		}
	}

	public Color channelWeights
	{
		get
		{
			return this._channelWeights;
		}
		set
		{
			if (this._channelWeights != value)
			{
				this.Set(ref this._channelWeights, value);
			}
		}
	}

	public float raycastThreshold
	{
		get
		{
			return this._raycastThreshold;
		}
		set
		{
			this._raycastThreshold = value;
		}
	}

	public bool invertMask
	{
		get
		{
			return this._invertMask;
		}
		set
		{
			if (this._invertMask != value)
			{
				this.Set(ref this._invertMask, value);
			}
		}
	}

	public bool invertOutsides
	{
		get
		{
			return this._invertOutsides;
		}
		set
		{
			if (this._invertOutsides != value)
			{
				this.Set(ref this._invertOutsides, value);
			}
		}
	}

	public bool isUsingRaycastFiltering => this._raycastThreshold > 0f;

	public bool isMaskingEnabled
	{
		get
		{
			if (base.isActiveAndEnabled)
			{
				return this.canvas;
			}
			return false;
		}
	}

	private RectTransform maskTransform
	{
		get
		{
			if (!this._maskTransform)
			{
				return this._maskTransform = (this._separateMask ? this._separateMask : base.GetComponent<RectTransform>());
			}
			return this._maskTransform;
		}
	}

	private Canvas canvas
	{
		get
		{
			if (!this._canvas)
			{
				return this._canvas = this.NearestEnabledCanvas();
			}
			return this._canvas;
		}
	}

	private bool isBasedOnGraphic => this._source == MaskSource.Graphic;

	bool ISoftMask.isAlive
	{
		get
		{
			if ((bool)this)
			{
				return !this._destroyed;
			}
			return false;
		}
	}

	public SoftMask()
	{
		this._materials = new MaterialReplacements(new MaterialReplacerChain(MaterialReplacer.globalReplacers, new MaterialReplacerImpl(this)), (Material m) =>
		{
			this._parameters.Apply(m);
		});
	}

	public Errors PollErrors()
	{
		return new Diagnostics(this).PollErrors();
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera cam)
	{
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.maskTransform, sp, cam, out var localPoint))
		{
			return false;
		}
		if (!Mathr.Inside(localPoint, this.LocalMaskRect(Vector4.zero)))
		{
			return this._invertOutsides;
		}
		if (!this._parameters.texture)
		{
			return true;
		}
		if (!this.isUsingRaycastFiltering)
		{
			return true;
		}
		if (!this._parameters.SampleMask(localPoint, out var mask))
		{
			Debug.LogErrorFormat(this, "Raycast Threshold greater than 0 can't be used on Soft Mask with texture '{0}' because it's not readable. You can make the texture readable in the Texture Import Settings.", this._parameters.activeTexture.name);
			return true;
		}
		if (this._invertMask)
		{
			mask = 1f - mask;
		}
		return mask >= this._raycastThreshold;
	}

	protected override void Start()
	{
		base.Start();
		this.WarnIfDefaultShaderIsNotSet();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		this.SubscribeOnWillRenderCanvases();
		this.SpawnMaskablesInChildren(base.transform);
		this.FindGraphic();
		if (this.isMaskingEnabled)
		{
			this.UpdateMaskParameters();
		}
		this.NotifyChildrenThatMaskMightChanged();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.UnsubscribeFromWillRenderCanvases();
		if ((bool)this._graphic)
		{
			this._graphic.UnregisterDirtyVerticesCallback(this.OnGraphicDirty);
			this._graphic.UnregisterDirtyMaterialCallback(this.OnGraphicDirty);
			this._graphic = null;
		}
		this.NotifyChildrenThatMaskMightChanged();
		this.DestroyMaterials();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this._destroyed = true;
		this.NotifyChildrenThatMaskMightChanged();
	}

	protected virtual void LateUpdate()
	{
		bool flag;
		flag = this.isMaskingEnabled;
		if (flag)
		{
			if (this._maskingWasEnabled != flag)
			{
				this.SpawnMaskablesInChildren(base.transform);
			}
			Graphic graphic;
			graphic = this._graphic;
			this.FindGraphic();
			if (this._lastMaskRect != this.maskTransform.rect || (object)this._graphic != graphic)
			{
				this._dirty = true;
			}
		}
		this._maskingWasEnabled = flag;
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		this._dirty = true;
	}

	protected override void OnDidApplyAnimationProperties()
	{
		base.OnDidApplyAnimationProperties();
		this._dirty = true;
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		this._canvas = null;
		this._dirty = true;
	}

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		this._canvas = null;
		this._dirty = true;
		this.NotifyChildrenThatMaskMightChanged();
	}

	private void OnTransformChildrenChanged()
	{
		this.SpawnMaskablesInChildren(base.transform);
	}

	private void SubscribeOnWillRenderCanvases()
	{
		SoftMask.Touch(CanvasUpdateRegistry.instance);
		Canvas.willRenderCanvases += this.OnWillRenderCanvases;
	}

	private void UnsubscribeFromWillRenderCanvases()
	{
		Canvas.willRenderCanvases -= this.OnWillRenderCanvases;
	}

	private void OnWillRenderCanvases()
	{
		if (this.isMaskingEnabled)
		{
			this.UpdateMaskParameters();
		}
	}

	private static T Touch<T>(T obj)
	{
		return obj;
	}

	Material ISoftMask.GetReplacement(Material original)
	{
		return this._materials.Get(original);
	}

	void ISoftMask.ReleaseReplacement(Material replacement)
	{
		this._materials.Release(replacement);
	}

	void ISoftMask.UpdateTransformChildren(Transform transform)
	{
		this.SpawnMaskablesInChildren(transform);
	}

	private void OnGraphicDirty()
	{
		if (this.isBasedOnGraphic)
		{
			this._dirty = true;
		}
	}

	private void FindGraphic()
	{
		if (!this._graphic && this.isBasedOnGraphic)
		{
			this._graphic = this.maskTransform.GetComponent<Graphic>();
			if ((bool)this._graphic)
			{
				this._graphic.RegisterDirtyVerticesCallback(this.OnGraphicDirty);
				this._graphic.RegisterDirtyMaterialCallback(this.OnGraphicDirty);
			}
		}
	}

	private Canvas NearestEnabledCanvas()
	{
		Canvas[] componentsInParent;
		componentsInParent = base.GetComponentsInParent<Canvas>(includeInactive: false);
		for (int i = 0; i < componentsInParent.Length; i = unchecked(i + 1))
		{
			if (componentsInParent[i].isActiveAndEnabled)
			{
				return componentsInParent[i];
			}
		}
		return null;
	}

	private void UpdateMaskParameters()
	{
		if (this._dirty || this.maskTransform.hasChanged)
		{
			this.CalculateMaskParameters();
			this.maskTransform.hasChanged = false;
			this._lastMaskRect = this.maskTransform.rect;
			this._dirty = false;
		}
		this._materials.ApplyAll();
	}

	private void SpawnMaskablesInChildren(Transform root)
	{
		using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
		{
			for (int i = 0; i < root.childCount; i = unchecked(i + 1))
			{
				Transform child;
				child = root.GetChild(i);
				child.GetComponents(SoftMask.s_maskables);
				if (SoftMask.s_maskables.Count == 0)
				{
					child.gameObject.AddComponent<SoftMaskable>();
				}
			}
		}
	}

	private void InvalidateChildren()
	{
		this.ForEachChildMaskable((SoftMaskable x) =>
		{
			x.Invalidate();
		});
	}

	private void NotifyChildrenThatMaskMightChanged()
	{
		this.ForEachChildMaskable((SoftMaskable x) =>
		{
			x.MaskMightChanged();
		});
	}

	private void ForEachChildMaskable(Action<SoftMaskable> f)
	{
		base.transform.GetComponentsInChildren(SoftMask.s_maskables);
		using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
		{
			for (int i = 0; i < SoftMask.s_maskables.Count; i = unchecked(i + 1))
			{
				SoftMaskable softMaskable;
				softMaskable = SoftMask.s_maskables[i];
				if ((bool)softMaskable && softMaskable.gameObject != base.gameObject)
				{
					f(softMaskable);
				}
			}
		}
	}

	private void DestroyMaterials()
	{
		this._materials.DestroyAllAndClear();
	}

	private SourceParameters DeduceSourceParameters()
	{
		SourceParameters result;
		result = default;
		switch (this._source)
		{
		case MaskSource.Graphic:
			if (this._graphic is Image)
			{
				result.image = (Image)this._graphic;
				result.sprite = result.image.sprite;
				result.spriteBorderMode = this.ToBorderMode(result.image.type);
				result.texture = (result.sprite ? result.sprite.texture : null);
			}
			else if (this._graphic is RawImage)
			{
				RawImage rawImage;
				rawImage = (RawImage)this._graphic;
				result.texture = rawImage.texture as Texture2D;
				result.textureUVRect = rawImage.uvRect;
			}
			break;
		case MaskSource.Sprite:
			result.sprite = this._sprite;
			result.spriteBorderMode = this._spriteBorderMode;
			result.texture = (result.sprite ? result.sprite.texture : null);
			break;
		case MaskSource.Texture:
			result.texture = this._texture;
			result.textureUVRect = this._textureUVRect;
			break;
		default:
			Debug.LogErrorFormat(this, "Unknown MaskSource: {0}", this._source);
			break;
		}
		return result;
	}

	private BorderMode ToBorderMode(Image.Type imageType)
	{
		switch (imageType)
		{
		case Image.Type.Simple:
			return BorderMode.Simple;
		case Image.Type.Sliced:
			return BorderMode.Sliced;
		case Image.Type.Tiled:
			return BorderMode.Tiled;
		default:
			Debug.LogErrorFormat(this, "SoftMask doesn't support image type {0}. Image type Simple will be used.", imageType);
			return BorderMode.Simple;
		}
	}

	private void CalculateMaskParameters()
	{
		SourceParameters sourceParameters;
		sourceParameters = this.DeduceSourceParameters();
		if ((bool)sourceParameters.sprite)
		{
			this.CalculateSpriteBased(sourceParameters.sprite, sourceParameters.spriteBorderMode);
		}
		else if ((bool)sourceParameters.texture)
		{
			this.CalculateTextureBased(sourceParameters.texture, sourceParameters.textureUVRect);
		}
		else
		{
			this.CalculateSolidFill();
		}
	}

	private void CalculateSpriteBased(Sprite sprite, BorderMode borderMode)
	{
		Sprite lastUsedSprite;
		lastUsedSprite = this._lastUsedSprite;
		this._lastUsedSprite = sprite;
		Errors errors;
		errors = Diagnostics.CheckSprite(sprite);
		if (errors != Errors.NoError)
		{
			if (lastUsedSprite != sprite)
			{
				this.WarnSpriteErrors(errors);
			}
			this.CalculateSolidFill();
			return;
		}
		if (!sprite)
		{
			this.CalculateSolidFill();
			return;
		}
		this.FillCommonParameters();
		Vector4 innerUV;
		innerUV = DataUtility.GetInnerUV(sprite);
		Vector4 outerUV;
		outerUV = DataUtility.GetOuterUV(sprite);
		Vector4 padding;
		padding = DataUtility.GetPadding(sprite);
		Vector4 vector;
		vector = this.LocalMaskRect(Vector4.zero);
		this._parameters.maskRectUV = outerUV;
		if (borderMode == BorderMode.Simple)
		{
			Vector4 v;
			v = Mathr.Div(padding, sprite.rect.size);
			this._parameters.maskRect = Mathr.ApplyBorder(vector, Mathr.Mul(v, Mathr.Size(vector)));
		}
		else
		{
			this._parameters.maskRect = Mathr.ApplyBorder(vector, padding * this.SpriteToCanvasScale(sprite));
			Vector4 border;
			border = SoftMask.AdjustBorders(sprite.border * this.SpriteToCanvasScale(sprite), vector);
			this._parameters.maskBorder = this.LocalMaskRect(border);
			this._parameters.maskBorderUV = innerUV;
		}
		this._parameters.texture = sprite.texture;
		this._parameters.borderMode = borderMode;
		if (borderMode == BorderMode.Tiled)
		{
			this._parameters.tileRepeat = this.MaskRepeat(sprite, this._parameters.maskBorder);
		}
	}

	private static Vector4 AdjustBorders(Vector4 border, Vector4 rect)
	{
		Vector2 vector;
		vector = Mathr.Size(rect);
		unchecked
		{
			for (int i = 0; i <= 1; i++)
			{
				float num;
				num = border[i] + border[i + 2];
				if (vector[i] < num && num != 0f)
				{
					float num2;
					num2 = vector[i] / num;
					border[i] *= num2;
					border[i + 2] *= num2;
				}
			}
			return border;
		}
	}

	private void CalculateTextureBased(Texture2D texture, Rect uvRect)
	{
		this.FillCommonParameters();
		this._parameters.maskRect = this.LocalMaskRect(Vector4.zero);
		this._parameters.maskRectUV = Mathr.ToVector(uvRect);
		this._parameters.texture = texture;
		this._parameters.borderMode = BorderMode.Simple;
	}

	private void CalculateSolidFill()
	{
		this.CalculateTextureBased(null, SoftMask.DefaultUVRect);
	}

	private void FillCommonParameters()
	{
		this._parameters.worldToMask = this.WorldToMask();
		this._parameters.maskChannelWeights = this._channelWeights;
		this._parameters.invertMask = this._invertMask;
		this._parameters.invertOutsides = this._invertOutsides;
	}

	private float SpriteToCanvasScale(Sprite sprite)
	{
		return (this.canvas ? this.canvas.referencePixelsPerUnit : 100f) / (sprite ? sprite.pixelsPerUnit : 100f);
	}

	private Matrix4x4 WorldToMask()
	{
		return this.maskTransform.worldToLocalMatrix * this.canvas.rootCanvas.transform.localToWorldMatrix;
	}

	private Vector4 LocalMaskRect(Vector4 border)
	{
		return Mathr.ApplyBorder(Mathr.ToVector(this.maskTransform.rect), border);
	}

	private Vector2 MaskRepeat(Sprite sprite, Vector4 centralPart)
	{
		return Mathr.Div(s: Mathr.Size(Mathr.ApplyBorder(Mathr.ToVector(sprite.rect), sprite.border)), v: Mathr.Size(centralPart) * this.SpriteToCanvasScale(sprite));
	}

	private void WarnIfDefaultShaderIsNotSet()
	{
		if (!this._defaultShader)
		{
			Debug.LogWarning("SoftMask may not work because its defaultShader is not set", this);
		}
	}

	private void WarnSpriteErrors(Errors errors)
	{
		if ((errors & Errors.TightPackedSprite) != 0)
		{
			Debug.LogError("SoftMask doesn't support tight packed sprites", this);
		}
		if ((errors & Errors.AlphaSplitSprite) != 0)
		{
			Debug.LogError("SoftMask doesn't support sprites with an alpha split texture", this);
		}
	}

	private void Set<T>(ref T field, T value)
	{
		field = value;
		this._dirty = true;
	}

	private void SetShader(ref Shader field, Shader value, bool warnIfNotSet = true)
	{
		if (field != value)
		{
			field = value;
			if (warnIfNotSet)
			{
				this.WarnIfDefaultShaderIsNotSet();
			}
			this.DestroyMaterials();
			this.InvalidateChildren();
		}
	}

	static SoftMask()
	{
		SoftMask.DefaultUVRect = new Rect(0f, 0f, 1f, 1f);
		SoftMask.s_masks = new List<SoftMask>();
		SoftMask.s_maskables = new List<SoftMaskable>();
	}
}
