using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(TeleportRenderer), PostProcessEvent.BeforeStack, "JDS/Post Process/Teleport", true)]
public sealed class Teleport : Ability
{
	public TextureParameter _Noise = new TextureParameter();

	public TextureParameter _DistortionNormals = new TextureParameter();
}
