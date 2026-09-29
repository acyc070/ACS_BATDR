using S13Audio;
using UnityEngine;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Dynamic", order = -5)]
public class S13ObjectDynamic : S13ObjectPrimitive<S13ObjectDynamic, S13HandlerDynamic>
{
	public override float startTime => 0f;
}
