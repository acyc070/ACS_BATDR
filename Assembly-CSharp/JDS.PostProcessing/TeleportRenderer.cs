using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class TeleportRenderer : PostProcessEffectRenderer<Teleport>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Teleport"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		if (base.settings._Noise.value != null)
		{
			propertySheet.properties.SetTexture("_Ink", base.settings._Noise);
		}
		if (base.settings._DistortionNormals.value != null)
		{
			propertySheet.properties.SetTexture("_DistortionNormals", base.settings._DistortionNormals);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
