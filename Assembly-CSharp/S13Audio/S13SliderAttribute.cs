using UnityEngine;

namespace S13Audio;

public class S13SliderAttribute : PropertyAttribute
{
	public string label;

	public float minValue;

	public float maxValue;

	public S13SliderAttribute(string label, float minValue, float maxValue)
	{
		this.label = label;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}
