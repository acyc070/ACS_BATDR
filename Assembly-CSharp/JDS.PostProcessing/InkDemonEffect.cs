using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(InkDemonRenderer), PostProcessEvent.AfterStack, "JDS/Post Process/Ink Demon Effect", true)]
public sealed class InkDemonEffect : Ability
{
	public TextureParameter _Distortion = new TextureParameter();

	public TextureParameter _DemonVignette = new TextureParameter();
}
