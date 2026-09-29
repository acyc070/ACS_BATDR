using UnityEngine;

public static class ShaderConstants
{
	public const string COLOR = "_Color";

	public const string POWER = "_Power";

	public const string POST_HIT_GLOW = "_PostHitGlow";

	public const float POST_HIT_GLOW_MIN = 0.3f;

	public const float POST_HIT_GLOW_MAX = 2f;

	public const string GRADIENT = "_Gradient";

	public const float GRADIENT_MIN = 0.1f;

	public const float GRADIENT_MAX = 1f;

	public const string GRADIENT_COLOR = "_GradientColor";

	public static Color GRADIENT_DEFAULT;

	public static Color GRADIENT_OUTLINE;

	static ShaderConstants()
	{
		ShaderConstants.GRADIENT_DEFAULT = new Color32(248, 194, 132, byte.MaxValue);
		ShaderConstants.GRADIENT_OUTLINE = new Color32(byte.MaxValue, 237, 85, byte.MaxValue);
	}
}
