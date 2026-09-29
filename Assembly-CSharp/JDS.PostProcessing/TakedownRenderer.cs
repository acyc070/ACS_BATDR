using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class TakedownRenderer : PostProcessEffectRenderer<Takedown>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Takedown"));
		if (base.settings._Cracks.value != null)
		{
			propertySheet.properties.SetTexture("_Cracks", base.settings._Cracks);
		}
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
