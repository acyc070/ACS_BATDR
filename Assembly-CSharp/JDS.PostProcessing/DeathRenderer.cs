using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class DeathRenderer : PostProcessEffectRenderer<Death>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Player Death"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		if (base.settings._Ink.value != null)
		{
			propertySheet.properties.SetTexture("_Ink", base.settings._Ink);
		}
		if (base.settings._DistortionNormals.value != null)
		{
			propertySheet.properties.SetTexture("_DistortionNormals", base.settings._DistortionNormals);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
