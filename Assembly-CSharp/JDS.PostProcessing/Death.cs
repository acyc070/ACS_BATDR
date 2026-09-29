using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(DeathRenderer), PostProcessEvent.AfterStack, "JDS/Post Process/Death", true)]
public sealed class Death : Ability
{
	public TextureParameter _Ink = new TextureParameter();

	public TextureParameter _DistortionNormals = new TextureParameter();
}
