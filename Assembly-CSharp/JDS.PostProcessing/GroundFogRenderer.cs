using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JDS.PostProcessing;

public sealed class GroundFogRenderer : PostProcessEffectRenderer<GroundFog>
{
	private float PreviousYEuler;

	private float Offset;

	private Transform CameraTransform;

	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet;
		propertySheet = context.propertySheets.Get(Shader.Find("JDS/Post Process/Ground Fog"));
		propertySheet.properties.SetFloat("_Power", base.settings._Power);
		propertySheet.properties.SetFloat("_Hardness", base.settings._Hardness);
		propertySheet.properties.SetFloat("_Height", base.settings._Height);
		propertySheet.properties.SetFloat("_Distance", base.settings._Distance);
		propertySheet.properties.SetColor("_Color", base.settings._Color);
		if (base.settings._Dust.value != null)
		{
			propertySheet.properties.SetTexture("_Dust", base.settings._Dust);
		}
		if (!this.CameraTransform)
		{
			this.CameraTransform = Camera.main.transform;
		}
		float y;
		y = this.CameraTransform.eulerAngles.y;
		if (y - this.PreviousYEuler > 90f)
		{
			this.Offset -= 360f;
		}
		else if (y - this.PreviousYEuler < -90f)
		{
			this.Offset += 360f;
		}
		this.PreviousYEuler = y;
		float num;
		num = this.CameraTransform.eulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		Vector3 vector;
		vector = new Vector3(0f - num, y + this.Offset, 0f) * 0.01f;
		propertySheet.properties.SetVector("_Rotation", vector);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
