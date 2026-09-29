using UnityEngine;

namespace SoftMasking;

public static class MaskChannel
{
	public static Color alpha;

	public static Color red;

	public static Color green;

	public static Color blue;

	public static Color gray;

	static MaskChannel()
	{
		MaskChannel.alpha = new Color(0f, 0f, 0f, 1f);
		MaskChannel.red = new Color(1f, 0f, 0f, 0f);
		MaskChannel.green = new Color(0f, 1f, 0f, 0f);
		MaskChannel.blue = new Color(0f, 0f, 1f, 0f);
		MaskChannel.gray = new Color(1f, 1f, 1f, 0f) / 3f;
	}
}
