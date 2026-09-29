using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Start-Loop-Stop", order = 5)]
public class S13ObjectPrimitiveStartLoopStop : S13ObjectPrimitiveMultiClip<S13ObjectPrimitiveStartLoopStop, S13HandlerPrimitiveStartLoopStop>
{
	[Space]
	public AudioClip startSegment;

	public AudioClip loopSegment;

	public AudioClip stopSegment;

	[Tooltip("If the start segment is playing, should it be cut off when the object is stopped?")]
	public bool cutStart;

	[Tooltip("If the stop segment is playing, should it be cut off when the object is played?")]
	public bool cutStop;
}
