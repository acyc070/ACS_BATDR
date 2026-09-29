using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13WeightedObjectList : IEnumerable<S13Object>, IEnumerable
{
	[Serializable]
	private class WeightedObjectList : ReorderableArray<S13WeightedObjectEntry>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true, relativeFieldSizes = new int[] { 4, 1, 1 })]
	public class S13WeightedObjectEntry : IS13HorizontalPropertyHolder, ISerializationCallbackReceiver
	{
		public S13Object obj;

		public float weight = 1f;

		[S13ReadOnlyField]
		public string __weightDisplay = "XXX%";

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.weight == 0f)
			{
				this.weight = 1f;
			}
		}
	}

	[SerializeField]
	[Reorderable(surrogateType = new Type[] { typeof(S13Object) }, surrogateProperty = new string[] { "obj" })]
	private WeightedObjectList objList = new WeightedObjectList();

	public void CalculateWeights()
	{
		if (this.objList.Count == 0)
		{
			return;
		}
		float num;
		num = Mathf.Max(this.objList.Sum((S13WeightedObjectEntry x) => x.weight), float.Epsilon);
		foreach (S13WeightedObjectEntry obj in this.objList)
		{
			obj.__weightDisplay = (obj.weight / num).ToString("P1");
		}
	}

	public S13Object GetRandom()
	{
		float num;
		num = global::UnityEngine.Random.Range(0f, Mathf.Max(this.objList.Sum((S13WeightedObjectEntry x) => x.weight), float.Epsilon));
		for (int num2 = 0; num2 < this.objList.Count; num2 = unchecked(num2 + 1))
		{
			if ((num -= this.objList[num2].weight) < 0f)
			{
				return this.objList[num2].obj;
			}
		}
		return null;
	}

	public S13Object[] GetObjects()
	{
		return this.objList?.Select((S13WeightedObjectEntry x) => x.obj).ToArray() ?? new S13Object[0];
	}

	public S13WeightedObjectEntry[] GetWeightedObjects()
	{
		return this.objList?.ToArray() ?? new S13WeightedObjectEntry[0];
	}

	public void SetObjects(S13Object[] objects)
	{
		if (this.objList == null)
		{
			this.objList = new WeightedObjectList();
		}
		this.objList.Clear();
		for (int i = 0; i < objects.Length; i = unchecked(i + 1))
		{
			this.objList.Add(new S13WeightedObjectEntry
			{
				obj = objects[i]
			});
		}
		this.CalculateWeights();
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13WeightedObjectEntry item in this.objList.Where((S13WeightedObjectEntry x) => x != null))
		{
			yield return item.obj;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13WeightedObjectEntry item in this.objList.Where((S13WeightedObjectEntry x) => x != null))
		{
			yield return item.obj;
		}
	}
}
