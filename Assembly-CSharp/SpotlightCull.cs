using UnityEngine;

[ExecuteAlways]
public class SpotlightCull : MonoBehaviour
{
	[SerializeField]
	private bool overrideShadowMatrix;

	[Range(0f, 100f)]
	[SerializeField]
	private float CullingFarClip = 100f;

	[Range(0.1f, 1f)]
	[SerializeField]
	private float AngleScale = 1f;

	[Min(0.1f)]
	[SerializeField]
	private float AspectRatio = 1f;
}
