using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(GroundFogRenderer), PostProcessEvent.AfterStack, "JDS/Post Process/Ground Fog", true)]
public sealed class GroundFog : Ability
{
	public TextureParameter _Dust = new TextureParameter();

	public ColorParameter _Color = new ColorParameter();

	[Range(0f, 2f)]
	public FloatParameter _Hardness = new FloatParameter();

	public FloatParameter _Height = new FloatParameter();

	[Range(1f, 100f)]
	public FloatParameter _Distance = new FloatParameter();
}
