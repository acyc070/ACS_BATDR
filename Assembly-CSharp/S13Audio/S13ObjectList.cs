using System;
using S13Audio.Malee;

namespace S13Audio;

[Serializable]
public class S13ObjectList : ReorderableArray<S13Object>, IS13Reorderable
{
	public S13ObjectList(int size)
		: base(size)
	{
	}
}
[Serializable]
public class S13ObjectList<TObject, THandler> : ReorderableArray<S13Object<TObject, THandler>> where TObject : S13Object<TObject, THandler> where THandler : S13Handler<TObject, THandler>
{
}
