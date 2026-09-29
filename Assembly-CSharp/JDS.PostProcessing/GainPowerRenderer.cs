using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class GainPowerRenderer : PostProcessEffectRenderer<GainPowerEffect>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Gain Power Effect"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		if (base.settings._Vigniette.value != null)
		{
			propertySheet.properties.SetTexture("_Vigniette", base.settings._Vigniette);
		}
		if (base.settings._Distortion.value != null)
		{
			propertySheet.properties.SetTexture("_Distortion", base.settings._Distortion);
		}
		_ = base.settings._Color.value;
		propertySheet.properties.SetColor("_Color", base.settings._Color);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
