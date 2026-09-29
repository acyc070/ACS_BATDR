using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class InkDemonRenderer : PostProcessEffectRenderer<InkDemonEffect>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Ink Demon Effect"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		if (base.settings._Distortion.value != null)
		{
			propertySheet.properties.SetTexture("_Distortion", base.settings._Distortion);
		}
		if (base.settings._DemonVignette.value != null)
		{
			propertySheet.properties.SetTexture("_DemonVignette", base.settings._DemonVignette);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
