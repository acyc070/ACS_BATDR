using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class DamageRenderer : PostProcessEffectRenderer<DamageEffect>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Damage Effect"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		if (base.settings._InkSplatter.value != null)
		{
			propertySheet.properties.SetTexture("_InkSplatter", base.settings._InkSplatter);
		}
		if (base.settings._Noise.value != null)
		{
			propertySheet.properties.SetTexture("_Noise", base.settings._Noise);
		}
		if (base.settings._FadeOverlay.value != null)
		{
			propertySheet.properties.SetTexture("_FadeOverlay", base.settings._FadeOverlay);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
