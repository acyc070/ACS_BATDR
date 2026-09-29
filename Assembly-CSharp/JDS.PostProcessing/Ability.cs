using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
public abstract class Ability : PostProcessEffectSettings
{
	[Range(0f, 1f)]
	public FloatParameter _Power = new FloatParameter
	{
		value = 0f
	};
}
