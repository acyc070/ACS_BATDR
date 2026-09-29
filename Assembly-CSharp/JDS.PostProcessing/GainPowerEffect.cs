using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(GainPowerRenderer), PostProcessEvent.BeforeStack, "JDS/Post Process/Gain Power", true)]
public sealed class GainPowerEffect : Ability
{
	public TextureParameter _Vigniette = new TextureParameter();

	public TextureParameter _Distortion = new TextureParameter();

	public ColorParameter _Color = new ColorParameter();
}
