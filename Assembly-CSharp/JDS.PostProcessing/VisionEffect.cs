using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(VisionRenderer), PostProcessEvent.BeforeStack, "JDS/Post Process/Vision", true)]
public sealed class VisionEffect : Ability
{
	public TextureParameter _FilmStrip = new TextureParameter();

	public TextureParameter _Noise = new TextureParameter();

	public TextureParameter _Dust = new TextureParameter();

	public TextureParameter _TransitionImage = new TextureParameter();

	public BoolParameter _Transition = new BoolParameter();
}
