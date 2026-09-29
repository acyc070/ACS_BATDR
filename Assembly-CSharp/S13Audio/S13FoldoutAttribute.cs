using System;

namespace S13Audio;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class S13FoldoutAttribute : Attribute
{
	public string Name;

	public string Header;

	public S13FoldoutAttribute(string name)
		: this(name, string.Empty)
	{
	}

	public S13FoldoutAttribute(string name, string header)
	{
		this.Name = name;
		this.Header = header;
	}
}
