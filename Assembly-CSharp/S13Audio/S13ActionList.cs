using System;
using S13Audio.Malee;

namespace S13Audio;

[Serializable]
public class S13ActionList<T> : ReorderableArray<T>, IS13Executable, IS13Reorderable where T : IS13Executable
{
	public bool IsExecutable => base.Length > 0;

	public void Execute()
	{
		foreach (T item in this)
		{
			item.Execute();
		}
	}
}
