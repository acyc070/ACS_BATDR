using UnityEngine;

public class ComponentRangeAttribute : PropertyAttribute
{
	public float min;

	public float max;

	public ComponentRangeAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
