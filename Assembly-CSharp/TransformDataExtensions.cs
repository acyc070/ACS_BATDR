using UnityEngine;

public static class TransformDataExtensions
{
	public static void SetData<T>(this T transform, TransformData transformData) where T : Transform
	{
		transform.position = transformData.Position;
		transform.rotation = transformData.Rotation;
		transform.eulerAngles = transformData.EulerAngles;
		transform.localScale = transformData.LocalScale;
	}
}
