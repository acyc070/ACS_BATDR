using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(DamageRenderer), PostProcessEvent.AfterStack, "JDS/Post Process/Damage", true)]
public sealed class DamageEffect : Ability
{
	public TextureParameter _InkSplatter = new TextureParameter();

	public TextureParameter _Noise = new TextureParameter();

	public TextureParameter _FadeOverlay = new TextureParameter();
}
