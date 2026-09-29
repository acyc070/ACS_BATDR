using UnityEngine;

public static class AspectRatioUtility
{
	private const float ASPECT_RATIO_MAX = 1.777f;

	private const float ASPECT_RATIO_MIN = 1.6f;

	private const float SCALE_MAX = 1f;

	private const float SCALE_MIN = 0.9f;

	public static float AspectRatio => (float)Screen.width / (float)Screen.height;

	public static float GetPercent()
	{
		return (AspectRatioUtility.AspectRatio - 1.6f) / 0.17699993f;
	}

	public static float GetScale()
	{
		return 0.9f + AspectRatioUtility.GetPercent() * 0.100000024f;
	}

	public static float GetScaledScreenHeight(float max, float min)
	{
		return min + (1f - AspectRatioUtility.GetPercent()) * (max - min);
	}

	public static Vector3 GetFullScale()
	{
		float scale;
		scale = AspectRatioUtility.GetScale();
		return new Vector3(scale, scale, scale);
	}
}
