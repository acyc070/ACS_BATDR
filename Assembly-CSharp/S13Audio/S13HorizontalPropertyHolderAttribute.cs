using System;
using UnityEngine;

namespace S13Audio;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class S13HorizontalPropertyHolderAttribute : PropertyAttribute
{
	private const string LABEL_DEFAULT = "DefaultLabel";

	public bool forceExpand;

	public bool indentLabel;

	public int[] relativeFieldSizes = new int[0];

	public string label = "DefaultLabel";

	public bool IsCustomLabel()
	{
		return this.label != "DefaultLabel";
	}
}
