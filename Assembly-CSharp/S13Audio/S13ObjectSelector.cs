using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Selector", order = 1)]
public class S13ObjectSelector : S13ObjectCompound<S13ObjectSelector, S13HandlerSelector>, IEnumerable<S13Object>, IEnumerable
{
	[FormerlySerializedAs("selectableObjects")]
	public S13WeightedObjectList selectableWeightedObjects = new S13WeightedObjectList();

	[Tooltip("The number of previous selections that should be avoided when randomizing the next choice. Setting to 0 will allow full randomness.")]
	public int avoidRepeatHistory;

	private void OnValidate()
	{
		this.avoidRepeatHistory = Mathf.Max(0, Mathf.Min(this.avoidRepeatHistory, unchecked(this.selectableWeightedObjects.Count((S13Object x) => x) - 1)));
		this.selectableWeightedObjects.CalculateWeights();
	}

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.selectableWeightedObjects;
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.selectableWeightedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13Object item in this.selectableWeightedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}
}
