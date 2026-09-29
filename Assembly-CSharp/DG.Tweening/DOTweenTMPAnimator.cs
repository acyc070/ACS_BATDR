using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace DG.Tweening;

public class DOTweenTMPAnimator : IDisposable
{
	private struct CharVertices(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight)
	{
		public Vector3 bottomLeft = bottomLeft;

		public Vector3 topLeft = topLeft;

		public Vector3 topRight = topRight;

		public Vector3 bottomRight = bottomRight;
	}

	private struct CharTransform
	{
		public int charIndex;

		public Vector3 offset;

		public Quaternion rotation;

		public Vector3 scale;

		private Vector3 _topLeftShift;

		private Vector3 _topRightShift;

		private Vector3 _bottomLeftShift;

		private Vector3 _bottomRightShift;

		private Vector3 _charMidBaselineOffset;

		private int _matIndex;

		private int _firstVertexIndex;

		private TMP_MeshInfo _meshInfo;

		public bool isVisible { get; private set; }

		public CharTransform(int charIndex, TMP_TextInfo textInfo, TMP_MeshInfo[] cachedMeshInfos)
		{
			this = default;
			this.charIndex = charIndex;
			this.offset = Vector3.zero;
			this.rotation = Quaternion.identity;
			this.scale = Vector3.one;
			this.Refresh(textInfo, cachedMeshInfos);
		}

		public void Refresh(TMP_TextInfo textInfo, TMP_MeshInfo[] cachedMeshInfos)
		{
			TMP_CharacterInfo tMP_CharacterInfo;
			tMP_CharacterInfo = textInfo.characterInfo[this.charIndex];
			bool flag;
			flag = tMP_CharacterInfo.character == ' ';
			this.isVisible = tMP_CharacterInfo.isVisible && !flag;
			this._matIndex = tMP_CharacterInfo.materialReferenceIndex;
			this._firstVertexIndex = tMP_CharacterInfo.vertexIndex;
			this._meshInfo = textInfo.meshInfo[this._matIndex];
			Vector3[] vertices;
			vertices = cachedMeshInfos[this._matIndex].vertices;
			this._charMidBaselineOffset = (flag ? Vector3.zero : ((vertices[this._firstVertexIndex] + vertices[unchecked(this._firstVertexIndex + 2)]) * 0.5f));
		}

		public void ResetAll(TMP_Text target, TMP_MeshInfo[] meshInfos, TMP_MeshInfo[] cachedMeshInfos)
		{
			this.ResetGeometry(target, cachedMeshInfos);
			this.ResetColors(target, meshInfos);
		}

		public void ResetTransformationData()
		{
			this.offset = Vector3.zero;
			this.rotation = Quaternion.identity;
			this.scale = Vector3.one;
			this._topLeftShift = (this._topRightShift = (this._bottomLeftShift = (this._bottomRightShift = Vector3.zero)));
		}

		public void ResetGeometry(TMP_Text target, TMP_MeshInfo[] cachedMeshInfos)
		{
			this.ResetTransformationData();
			Vector3[] vertices;
			vertices = this._meshInfo.vertices;
			Vector3[] vertices2;
			vertices2 = cachedMeshInfos[this._matIndex].vertices;
			vertices[this._firstVertexIndex] = vertices2[this._firstVertexIndex];
			unchecked
			{
				vertices[this._firstVertexIndex + 1] = vertices2[this._firstVertexIndex + 1];
				vertices[this._firstVertexIndex + 2] = vertices2[this._firstVertexIndex + 2];
				vertices[this._firstVertexIndex + 3] = vertices2[this._firstVertexIndex + 3];
				this._meshInfo.mesh.vertices = this._meshInfo.vertices;
				target.UpdateGeometry(this._meshInfo.mesh, this._matIndex);
			}
		}

		public void ResetColors(TMP_Text target, TMP_MeshInfo[] meshInfos)
		{
			Color color;
			color = target.color;
			Color32[] colors;
			colors = meshInfos[this._matIndex].colors32;
			colors[this._firstVertexIndex] = color;
			unchecked
			{
				colors[this._firstVertexIndex + 1] = color;
				colors[this._firstVertexIndex + 2] = color;
				colors[this._firstVertexIndex + 3] = color;
				target.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
			}
		}

		public Color32 GetColor(TMP_MeshInfo[] meshInfos)
		{
			return meshInfos[this._matIndex].colors32[this._firstVertexIndex];
		}

		public CharVertices GetVertices()
		{
			return unchecked(new CharVertices(this._meshInfo.vertices[this._firstVertexIndex], this._meshInfo.vertices[this._firstVertexIndex + 1], this._meshInfo.vertices[this._firstVertexIndex + 2], this._meshInfo.vertices[this._firstVertexIndex + 3]));
		}

		public void UpdateAlpha(TMP_Text target, Color alphaColor, TMP_MeshInfo[] meshInfos, bool apply = true)
		{
			unchecked
			{
				byte a;
				a = (byte)(alphaColor.a * 255f);
				Color32[] colors;
				colors = meshInfos[this._matIndex].colors32;
				colors[this._firstVertexIndex].a = a;
				colors[this._firstVertexIndex + 1].a = a;
				colors[this._firstVertexIndex + 2].a = a;
				colors[this._firstVertexIndex + 3].a = a;
				if (apply)
				{
					target.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
				}
			}
		}

		public void UpdateColor(TMP_Text target, Color32 color, TMP_MeshInfo[] meshInfos, bool apply = true)
		{
			Color32[] colors;
			colors = meshInfos[this._matIndex].colors32;
			colors[this._firstVertexIndex] = color;
			unchecked
			{
				colors[this._firstVertexIndex + 1] = color;
				colors[this._firstVertexIndex + 2] = color;
				colors[this._firstVertexIndex + 3] = color;
				if (apply)
				{
					target.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
				}
			}
		}

		public void UpdateGeometry(TMP_Text target, Vector3 offset, Quaternion rotation, Vector3 scale, TMP_MeshInfo[] cachedMeshInfos, bool apply = true)
		{
			this.offset = offset;
			this.rotation = rotation;
			this.scale = scale;
			unchecked
			{
				if (apply)
				{
					Vector3[] vertices;
					vertices = this._meshInfo.vertices;
					Vector3[] vertices2;
					vertices2 = cachedMeshInfos[this._matIndex].vertices;
					vertices[this._firstVertexIndex] = vertices2[this._firstVertexIndex] - this._charMidBaselineOffset;
					vertices[this._firstVertexIndex + 1] = vertices2[this._firstVertexIndex + 1] - this._charMidBaselineOffset;
					vertices[this._firstVertexIndex + 2] = vertices2[this._firstVertexIndex + 2] - this._charMidBaselineOffset;
					vertices[this._firstVertexIndex + 3] = vertices2[this._firstVertexIndex + 3] - this._charMidBaselineOffset;
					Matrix4x4 matrix4x;
					matrix4x = Matrix4x4.TRS(this.offset, this.rotation, this.scale);
					vertices[this._firstVertexIndex] = matrix4x.MultiplyPoint3x4(vertices[this._firstVertexIndex]) + this._charMidBaselineOffset + this._bottomLeftShift;
					vertices[this._firstVertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[this._firstVertexIndex + 1]) + this._charMidBaselineOffset + this._topLeftShift;
					vertices[this._firstVertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[this._firstVertexIndex + 2]) + this._charMidBaselineOffset + this._topRightShift;
					vertices[this._firstVertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[this._firstVertexIndex + 3]) + this._charMidBaselineOffset + this._bottomRightShift;
					this._meshInfo.mesh.vertices = this._meshInfo.vertices;
					target.UpdateGeometry(this._meshInfo.mesh, this._matIndex);
				}
			}
		}

		public void ShiftVertices(TMP_Text target, Vector3 topLeftShift, Vector3 topRightShift, Vector3 bottomLeftShift, Vector3 bottomRightShift)
		{
			this._topLeftShift += topLeftShift;
			this._topRightShift += topRightShift;
			this._bottomLeftShift += bottomLeftShift;
			this._bottomRightShift += bottomRightShift;
			Vector3[] vertices;
			vertices = this._meshInfo.vertices;
			vertices[this._firstVertexIndex] = vertices[this._firstVertexIndex] + this._bottomLeftShift;
			unchecked
			{
				vertices[this._firstVertexIndex + 1] = vertices[this._firstVertexIndex + 1] + this._topLeftShift;
				vertices[this._firstVertexIndex + 2] = vertices[this._firstVertexIndex + 2] + this._topRightShift;
				vertices[this._firstVertexIndex + 3] = vertices[this._firstVertexIndex + 3] + this._bottomRightShift;
				this._meshInfo.mesh.vertices = this._meshInfo.vertices;
				target.UpdateGeometry(this._meshInfo.mesh, this._matIndex);
			}
		}

		public void ResetVerticesShift(TMP_Text target)
		{
			Vector3[] vertices;
			vertices = this._meshInfo.vertices;
			vertices[this._firstVertexIndex] = vertices[this._firstVertexIndex] - this._bottomLeftShift;
			unchecked
			{
				vertices[this._firstVertexIndex + 1] = vertices[this._firstVertexIndex + 1] - this._topLeftShift;
				vertices[this._firstVertexIndex + 2] = vertices[this._firstVertexIndex + 2] - this._topRightShift;
				vertices[this._firstVertexIndex + 3] = vertices[this._firstVertexIndex + 3] - this._bottomRightShift;
				this._meshInfo.mesh.vertices = this._meshInfo.vertices;
				target.UpdateGeometry(this._meshInfo.mesh, this._matIndex);
				this._topLeftShift = (this._topRightShift = (this._bottomLeftShift = (this._bottomRightShift = Vector3.zero)));
			}
		}
	}

	private static readonly Dictionary<TMP_Text, DOTweenTMPAnimator> _targetToAnimator;

	private readonly List<CharTransform> _charTransforms = new List<CharTransform>();

	private TMP_MeshInfo[] _cachedMeshInfos;

	private bool _ignoreTextChangedEvent;

	public TMP_Text target { get; private set; }

	public TMP_TextInfo textInfo { get; private set; }

	public DOTweenTMPAnimator(TMP_Text target)
	{
		if (target == null)
		{
			Debugger.LogError("DOTweenTMPAnimator target can't be null");
			return;
		}
		if (!target.gameObject.activeInHierarchy)
		{
			Debugger.LogError("You can't create a DOTweenTMPAnimator if its target is disabled");
			return;
		}
		if (DOTweenTMPAnimator._targetToAnimator.ContainsKey(target))
		{
			if (Debugger.logPriority >= 2)
			{
				Debugger.Log($"A DOTweenTMPAnimator for \"{target}\" already exists: disposing it because you can't have more than one DOTweenTMPAnimator for the same TextMesh Pro object. If you have tweens running on the disposed DOTweenTMPAnimator you should kill them manually");
			}
			DOTweenTMPAnimator._targetToAnimator[target].Dispose();
			DOTweenTMPAnimator._targetToAnimator.Remove(target);
		}
		this.target = target;
		DOTweenTMPAnimator._targetToAnimator.Add(target, this);
		this.Refresh();
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(this.OnTextChanged);
	}

	public static void DisposeInstanceFor(TMP_Text target)
	{
		if (DOTweenTMPAnimator._targetToAnimator.ContainsKey(target))
		{
			DOTweenTMPAnimator._targetToAnimator[target].Dispose();
			DOTweenTMPAnimator._targetToAnimator.Remove(target);
		}
	}

	public void Dispose()
	{
		this.target = null;
		this._charTransforms.Clear();
		this.textInfo = null;
		this._cachedMeshInfos = null;
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(this.OnTextChanged);
	}

	public void Refresh()
	{
		this._ignoreTextChangedEvent = true;
		this.target.ForceMeshUpdate(ignoreActiveState: true);
		this.textInfo = this.target.textInfo;
		this._cachedMeshInfos = this.textInfo.CopyMeshInfoVertexData();
		int characterCount;
		characterCount = this.textInfo.characterCount;
		int num;
		num = this._charTransforms.Count;
		unchecked
		{
			if (num > characterCount)
			{
				this._charTransforms.RemoveRange(characterCount, num - characterCount);
				num = characterCount;
			}
			for (int i = 0; i < num; i++)
			{
				CharTransform value;
				value = this._charTransforms[i];
				value.ResetTransformationData();
				value.Refresh(this.textInfo, this._cachedMeshInfos);
				this._charTransforms[i] = value;
			}
			for (int j = num; j < characterCount; j++)
			{
				this._charTransforms.Add(new CharTransform(j, this.textInfo, this._cachedMeshInfos));
			}
			this._ignoreTextChangedEvent = false;
		}
	}

	public void Reset()
	{
		int count;
		count = this._charTransforms.Count;
		for (int i = 0; i < count; i = unchecked(i + 1))
		{
			this._charTransforms[i].ResetAll(this.target, this.textInfo.meshInfo, this._cachedMeshInfos);
		}
	}

	private void OnTextChanged(global::UnityEngine.Object obj)
	{
		if (!this._ignoreTextChangedEvent && !(this.target == null) && !(obj != this.target))
		{
			this.Refresh();
		}
	}

	private bool ValidateChar(int charIndex, bool isTween = true)
	{
		if (this.textInfo.characterCount <= charIndex)
		{
			Debugger.LogError($"CharIndex {charIndex} doesn't exist");
			return false;
		}
		if (!this.textInfo.characterInfo[charIndex].isVisible)
		{
			if (Debugger.logPriority > 1)
			{
				if (isTween)
				{
					Debugger.Log($"CharIndex {charIndex} isn't visible, ignoring it and returning an empty tween (TextMesh Pro will behave weirdly if invisible chars are included in the animation)");
				}
				else
				{
					Debugger.Log($"CharIndex {charIndex} isn't visible, ignoring it");
				}
			}
			return false;
		}
		return true;
	}

	private bool ValidateSpan(int fromCharIndex, int toCharIndex, out int firstVisibleCharIndex, out int lastVisibleCharIndex)
	{
		firstVisibleCharIndex = -1;
		lastVisibleCharIndex = -1;
		int characterCount;
		characterCount = this.textInfo.characterCount;
		if (fromCharIndex >= characterCount)
		{
			return false;
		}
		unchecked
		{
			if (toCharIndex >= characterCount)
			{
				toCharIndex = characterCount - 1;
			}
			for (int i = fromCharIndex; i < toCharIndex + 1; i++)
			{
				if (this._charTransforms[i].isVisible)
				{
					firstVisibleCharIndex = i;
					break;
				}
			}
			if (firstVisibleCharIndex == -1)
			{
				return false;
			}
			for (int num = toCharIndex; num > firstVisibleCharIndex - 1; num--)
			{
				if (this._charTransforms[num].isVisible)
				{
					lastVisibleCharIndex = num;
					break;
				}
			}
			if (lastVisibleCharIndex == -1)
			{
				return false;
			}
			return true;
		}
	}

	public void SkewSpanX(int fromCharIndex, int toCharIndex, float skewFactor, bool skewTop = true)
	{
		if (!this.ValidateSpan(fromCharIndex, toCharIndex, out var firstVisibleCharIndex, out var lastVisibleCharIndex))
		{
			return;
		}
		unchecked
		{
			for (int i = firstVisibleCharIndex; i < lastVisibleCharIndex + 1; i++)
			{
				if (this._charTransforms[i].isVisible)
				{
					this._charTransforms[i].GetVertices();
					this.SkewCharX(i, skewFactor, skewTop);
				}
			}
		}
	}

	public void SkewSpanY(int fromCharIndex, int toCharIndex, float skewFactor, TMPSkewSpanMode mode = TMPSkewSpanMode.Default, bool skewRight = true)
	{
		if (!this.ValidateSpan(fromCharIndex, toCharIndex, out var firstVisibleCharIndex, out var lastVisibleCharIndex))
		{
			return;
		}
		if (mode == TMPSkewSpanMode.AsMaxSkewFactor)
		{
			CharVertices vertices;
			vertices = this._charTransforms[firstVisibleCharIndex].GetVertices();
			CharVertices vertices2;
			vertices2 = this._charTransforms[lastVisibleCharIndex].GetVertices();
			float num;
			num = Mathf.Abs(vertices2.bottomRight.x - vertices.bottomLeft.x);
			skewFactor *= Mathf.Abs(vertices2.topRight.y - vertices2.bottomRight.y) / num;
		}
		float num2;
		num2 = 0f;
		CharVertices charVertices;
		charVertices = default;
		float num3;
		num3 = 0f;
		unchecked
		{
			if (skewRight)
			{
				for (int i = firstVisibleCharIndex; i < lastVisibleCharIndex + 1; i++)
				{
					if (this._charTransforms[i].isVisible)
					{
						CharVertices vertices3;
						vertices3 = this._charTransforms[i].GetVertices();
						float num4;
						num4 = this.SkewCharY(i, skewFactor, skewRight);
						if (i > firstVisibleCharIndex)
						{
							float num5;
							num5 = Mathf.Abs(charVertices.bottomLeft.x - charVertices.bottomRight.x);
							num2 += num3 + num3 * Mathf.Abs(vertices3.bottomLeft.x - charVertices.bottomRight.x) / num5;
							this.SetCharOffset(i, new Vector3(0f, this._charTransforms[i].offset.y + num2, 0f));
						}
						charVertices = vertices3;
						num3 = num4;
					}
				}
				return;
			}
			for (int num6 = lastVisibleCharIndex; num6 > firstVisibleCharIndex - 1; num6--)
			{
				if (this._charTransforms[num6].isVisible)
				{
					CharVertices vertices4;
					vertices4 = this._charTransforms[num6].GetVertices();
					float num7;
					num7 = this.SkewCharY(num6, skewFactor, skewRight);
					if (num6 < lastVisibleCharIndex)
					{
						float num8;
						num8 = Mathf.Abs(charVertices.bottomLeft.x - charVertices.bottomRight.x);
						num2 += num3 + num3 * Mathf.Abs(vertices4.bottomRight.x - charVertices.bottomLeft.x) / num8;
						this.SetCharOffset(num6, new Vector3(0f, this._charTransforms[num6].offset.y + num2, 0f));
					}
					charVertices = vertices4;
					num3 = num7;
				}
			}
		}
	}

	public Color GetCharColor(int charIndex)
	{
		if (!this.ValidateChar(charIndex))
		{
			return Color.white;
		}
		return this._charTransforms[charIndex].GetColor(this.textInfo.meshInfo);
	}

	public Vector3 GetCharOffset(int charIndex)
	{
		if (!this.ValidateChar(charIndex))
		{
			return Vector3.zero;
		}
		return this._charTransforms[charIndex].offset;
	}

	public Vector3 GetCharRotation(int charIndex)
	{
		if (!this.ValidateChar(charIndex))
		{
			return Vector3.zero;
		}
		return this._charTransforms[charIndex].rotation.eulerAngles;
	}

	public Vector3 GetCharScale(int charIndex)
	{
		if (!this.ValidateChar(charIndex))
		{
			return Vector3.zero;
		}
		return this._charTransforms[charIndex].scale;
	}

	public void SetCharColor(int charIndex, Color32 color)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateColor(this.target, color, this.textInfo.meshInfo);
			this._charTransforms[charIndex] = value;
		}
	}

	public void SetCharOffset(int charIndex, Vector3 offset)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, offset, value.rotation, value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}
	}

	public void SetCharRotation(int charIndex, Vector3 rotation)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, Quaternion.Euler(rotation), value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}
	}

	public void SetCharScale(int charIndex, Vector3 scale)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, value.rotation, scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}
	}

	public void ShiftCharVertices(int charIndex, Vector3 topLeftShift, Vector3 topRightShift, Vector3 bottomLeftShift, Vector3 bottomRightShift)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.ShiftVertices(this.target, topLeftShift, topRightShift, bottomLeftShift, bottomRightShift);
			this._charTransforms[charIndex] = value;
		}
	}

	public float SkewCharX(int charIndex, float skewFactor, bool skewTop = true)
	{
		if (!this.ValidateChar(charIndex))
		{
			return 0f;
		}
		Vector3 vector;
		vector = new Vector3(skewFactor, 0f, 0f);
		CharTransform value;
		value = this._charTransforms[charIndex];
		if (skewTop)
		{
			value.ShiftVertices(this.target, vector, vector, Vector3.zero, Vector3.zero);
		}
		else
		{
			value.ShiftVertices(this.target, Vector3.zero, Vector3.zero, vector, vector);
		}
		this._charTransforms[charIndex] = value;
		return skewFactor;
	}

	public float SkewCharY(int charIndex, float skewFactor, bool skewRight = true, bool fixedSkew = false)
	{
		if (!this.ValidateChar(charIndex))
		{
			return 0f;
		}
		float num;
		num = (fixedSkew ? skewFactor : (skewFactor * this.textInfo.characterInfo[charIndex].aspectRatio));
		Vector3 vector;
		vector = new Vector3(0f, num, 0f);
		CharTransform value;
		value = this._charTransforms[charIndex];
		if (skewRight)
		{
			value.ShiftVertices(this.target, Vector3.zero, vector, Vector3.zero, vector);
		}
		else
		{
			value.ShiftVertices(this.target, vector, Vector3.zero, vector, Vector3.zero);
		}
		this._charTransforms[charIndex] = value;
		return num;
	}

	public void ResetVerticesShift(int charIndex)
	{
		if (this.ValidateChar(charIndex))
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.ResetVerticesShift(this.target);
			this._charTransforms[charIndex] = value;
		}
	}

	public TweenerCore<Color, Color, ColorOptions> DOFadeChar(int charIndex, float endValue, float duration)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		return DOTween.ToAlpha(() => this._charTransforms[charIndex].GetColor(this.textInfo.meshInfo), (Color x) =>
		{
			this._charTransforms[charIndex].UpdateAlpha(this.target, x, this.textInfo.meshInfo);
		}, endValue, duration);
	}

	public TweenerCore<Color, Color, ColorOptions> DOColorChar(int charIndex, Color endValue, float duration)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		return DOTween.To(() => this._charTransforms[charIndex].GetColor(this.textInfo.meshInfo), (Color x) =>
		{
			this._charTransforms[charIndex].UpdateColor(this.target, x, this.textInfo.meshInfo);
		}, endValue, duration);
	}

	public TweenerCore<Vector3, Vector3, VectorOptions> DOOffsetChar(int charIndex, Vector3 endValue, float duration)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		return DOTween.To(() => this._charTransforms[charIndex].offset, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, x, value.rotation, value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, endValue, duration);
	}

	public TweenerCore<Quaternion, Vector3, QuaternionOptions> DORotateChar(int charIndex, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		TweenerCore<Quaternion, Vector3, QuaternionOptions> tweenerCore;
		tweenerCore = DOTween.To(() => this._charTransforms[charIndex].rotation, (Quaternion x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, x, value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, endValue, duration);
		tweenerCore.plugOptions.rotateMode = mode;
		return tweenerCore;
	}

	public TweenerCore<Vector3, Vector3, VectorOptions> DOScaleChar(int charIndex, float endValue, float duration)
	{
		return this.DOScaleChar(charIndex, new Vector3(endValue, endValue, endValue), duration);
	}

	public TweenerCore<Vector3, Vector3, VectorOptions> DOScaleChar(int charIndex, Vector3 endValue, float duration)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		return DOTween.To(() => this._charTransforms[charIndex].scale, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, value.rotation, x, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, endValue, duration);
	}

	public Tweener DOPunchCharOffset(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Punch(() => this._charTransforms[charIndex].offset, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, x, value.rotation, value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, punch, duration, vibrato, elasticity);
	}

	public Tweener DOPunchCharRotation(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Punch(() => this._charTransforms[charIndex].rotation.eulerAngles, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, Quaternion.Euler(x), value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, punch, duration, vibrato, elasticity);
	}

	public Tweener DOPunchCharScale(int charIndex, float punch, float duration, int vibrato = 10, float elasticity = 1f)
	{
		return this.DOPunchCharScale(charIndex, new Vector3(punch, punch, punch), duration, vibrato, elasticity);
	}

	public Tweener DOPunchCharScale(int charIndex, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Punch(() => this._charTransforms[charIndex].scale, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, value.rotation, x, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, punch, duration, vibrato, elasticity);
	}

	public Tweener DOShakeCharOffset(int charIndex, float duration, float strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
	{
		return this.DOShakeCharOffset(charIndex, duration, new Vector3(strength, strength, strength), vibrato, randomness, fadeOut);
	}

	public Tweener DOShakeCharOffset(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Shake(() => this._charTransforms[charIndex].offset, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, x, value.rotation, value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, duration, strength, vibrato, randomness, fadeOut);
	}

	public Tweener DOShakeCharRotation(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Shake(() => this._charTransforms[charIndex].rotation.eulerAngles, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, Quaternion.Euler(x), value.scale, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, duration, strength, vibrato, randomness, fadeOut);
	}

	public Tweener DOShakeCharScale(int charIndex, float duration, float strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
	{
		return this.DOShakeCharScale(charIndex, duration, new Vector3(strength, strength, strength), vibrato, randomness, fadeOut);
	}

	public Tweener DOShakeCharScale(int charIndex, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
	{
		if (!this.ValidateChar(charIndex))
		{
			return null;
		}
		if (duration <= 0f)
		{
			if (Debugger.logPriority > 0)
			{
				Debug.LogWarning("Duration can't be 0, returning NULL without creating a tween");
			}
			return null;
		}
		return DOTween.Shake(() => this._charTransforms[charIndex].scale, (Vector3 x) =>
		{
			CharTransform value;
			value = this._charTransforms[charIndex];
			value.UpdateGeometry(this.target, value.offset, value.rotation, x, this._cachedMeshInfos);
			this._charTransforms[charIndex] = value;
		}, duration, strength, vibrato, randomness, fadeOut);
	}

	static DOTweenTMPAnimator()
	{
		DOTweenTMPAnimator._targetToAnimator = new Dictionary<TMP_Text, DOTweenTMPAnimator>();
	}
}
