using System;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

[Serializable]
[PostProcess(typeof(TakedownRenderer), PostProcessEvent.BeforeStack, "JDS/Post Process/Takedown", true)]
public sealed class Takedown : Ability
{
	public TextureParameter _Cracks = new TextureParameter();
}
