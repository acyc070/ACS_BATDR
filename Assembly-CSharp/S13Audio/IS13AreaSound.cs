using System;
using UnityEngine;

namespace S13Audio;

public interface IS13AreaSound
{
	Vector3 targetPosition { set; }

	bool targetWithinTrigger { get; }

	event Action onAreaEnter;

	event Action onAreaExit;
}
