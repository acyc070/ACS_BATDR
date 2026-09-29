using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public string buttonName;

	public InspectorButtonAttribute(string buttonName = null)
	{
		this.buttonName = buttonName;
	}
}
