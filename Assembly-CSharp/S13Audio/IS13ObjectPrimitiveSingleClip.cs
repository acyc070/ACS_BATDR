using UnityEngine;

namespace S13Audio;

public interface IS13ObjectPrimitiveSingleClip : IS13ObjectPrimitive, IS13Controllable
{
	AudioClip GetClip();

	void SetClip(AudioClip clip);
}
