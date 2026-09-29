using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Container", order = 0)]
public class S13ObjectContainer : S13ObjectCompound<S13ObjectContainer, S13HandlerContainer>, IEnumerable<S13Object>, IEnumerable
{
	public S13Object[] containedObjects = new S13Object[0];

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.containedObjects;
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.containedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13Object item in this.containedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}
}
