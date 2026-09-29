using System;
using UnityEngine;

namespace S13Audio;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class S13CurveRangeAttribute : PropertyAttribute
{
	private static Color defaultColor;

	public Vector2 Min { get; private set; }

	public Vector2 Max { get; private set; }

	public Color Color { get; private set; }

	public S13CurveRangeAttribute(Vector2 min, Vector2 max, Color color)
	{
		this.Min = min;
		this.Max = max;
		this.Color = color;
	}

	public S13CurveRangeAttribute(Vector2 min, Vector2 max)
		: this(min, max, S13CurveRangeAttribute.defaultColor)
	{
	}

	public S13CurveRangeAttribute(Color color)
		: this(Vector2.zero, Vector2.one, color)
	{
	}

	public S13CurveRangeAttribute(float minX, float minY, float maxX, float maxY)
		: this(new Vector2(minX, minY), new Vector2(maxX, maxY), S13CurveRangeAttribute.defaultColor)
	{
	}

	public S13CurveRangeAttribute(float minX, float minY, float maxX, float maxY, Color color)
		: this(new Vector2(minX, minY), new Vector2(maxX, maxY), color)
	{
	}

	static S13CurveRangeAttribute()
	{
		S13CurveRangeAttribute.defaultColor = Color.green;
	}
}
