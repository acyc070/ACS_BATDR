using UnityEngine;

public interface IHittable
{
	bool IsPlayerBreakable { get; }

	bool IsBroken { get; }

	void Hit(RaycastHit hit);
}
