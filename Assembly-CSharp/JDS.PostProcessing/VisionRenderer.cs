using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class VisionRenderer : PostProcessEffectRenderer<VisionEffect>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Vision Effect"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		propertySheet.properties.SetFloat("_Transition", (!base.settings._Transition) ? 0f : 1f);
		if (base.settings._FilmStrip.value != null)
		{
			propertySheet.properties.SetTexture("_FilmStrip", base.settings._FilmStrip);
		}
		if (base.settings._Noise.value != null)
		{
			propertySheet.properties.SetTexture("_Noise", base.settings._Noise);
		}
		if (base.settings._Dust.value != null)
		{
			propertySheet.properties.SetTexture("_Dust", base.settings._Dust);
		}
		if (base.settings._TransitionImage.value != null)
		{
			propertySheet.properties.SetTexture("_TransitionImage", base.settings._TransitionImage);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
