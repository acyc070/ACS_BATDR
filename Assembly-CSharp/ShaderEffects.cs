using DG.Tweening;
using UnityEngine;

public static class ShaderEffects
{
	public static void SetInt(string name, int value, params object[] renderers)
	{
		ShaderEffects.InternalSetInt(name, value, renderers);
	}

	private static void InternalSetInt(string _name, int _value, params object[] _renderers)
	{
		if (_renderers == null || _renderers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _renderers.Length; i = unchecked(i + 1))
		{
			Material material;
			material = ShaderEffects.GetMaterial(_renderers[i]);
			if (material != null && material.GetInt(_name) != _value)
			{
				material.SetInt(_name, _value);
			}
		}
	}

	public static void SetFloat(string name, float value, params object[] renderers)
	{
		ShaderEffects.InternalSetFloat(name, value, renderers);
	}

	private static void InternalSetFloat(string _name, float _value, params object[] _renderers)
	{
		if (_renderers == null || _renderers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _renderers.Length; i = unchecked(i + 1))
		{
			Material material;
			material = ShaderEffects.GetMaterial(_renderers[i]);
			if (material != null && material.GetFloat(_name) != _value)
			{
				DOTween.Kill(material.GetHashCode());
				material.SetFloat(_name, _value);
			}
		}
	}

	public static void SetColor(string name, Color value, params object[] renderers)
	{
		ShaderEffects.InternalSetColor(name, value, renderers);
	}

	private static void InternalSetColor(string _name, Color _value, params object[] _renderers)
	{
		if (_renderers == null || _renderers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _renderers.Length; i = unchecked(i + 1))
		{
			Material material;
			material = ShaderEffects.GetMaterial(_renderers[i]);
			if (material != null && material.GetColor(_name) != _value)
			{
				material.SetColor(_name, _value);
			}
		}
	}

	public static void Fade(string name, float startValue, float endValue, float duration, params object[] renderers)
	{
		ShaderEffects.InternalFade(name, startValue, endValue, duration, renderers);
	}

	private static void InternalFade(string _name, float _startValue, float _endValue, float _duration, params object[] _renderers)
	{
		if (_renderers != null && _renderers.Length != 0)
		{
			for (int i = 0; i < _renderers.Length; i = unchecked(i + 1))
			{
				ShaderEffects.TweenValue(_name, _startValue, _endValue, _duration, ShaderEffects.GetMaterial(_renderers[i]));
			}
		}
	}

	public static void TweenValue(string _name, float _startValue, float _endValue, float _duration, Material _material)
	{
		if (!(_material == null))
		{
			int hashCode;
			hashCode = _material.GetHashCode();
			float tween;
			tween = _startValue;
			DOTween.Kill(hashCode);
			_material.SetFloat(_name, _startValue);
			DOTween.To(() => tween, (float value) =>
			{
				tween = value;
			}, _endValue, _duration).SetId(hashCode).OnUpdate(() =>
			{
				_material.SetFloat(_name, tween);
			});
		}
	}

	private static Material GetMaterial(object _renderer)
	{
		Material result;
		result = null;
		if (_renderer != null)
		{
			if (_renderer.GetType() == typeof(Material))
			{
				result = _renderer as Material;
			}
			else if (_renderer.GetType() == typeof(MeshRenderer))
			{
				result = (_renderer as MeshRenderer).material;
			}
			else if (_renderer.GetType() == typeof(SkinnedMeshRenderer))
			{
				result = (_renderer as SkinnedMeshRenderer).material;
			}
		}
		return result;
	}
}
