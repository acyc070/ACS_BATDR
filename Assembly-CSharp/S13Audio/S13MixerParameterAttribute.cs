using UnityEngine;

namespace S13Audio;

public class S13MixerParameterAttribute : PropertyAttribute
{
	public string fieldName;

	public S13MixerParameterAttribute(string fieldName)
	{
		this.fieldName = fieldName;
	}
}
