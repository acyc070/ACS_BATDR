using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Sequence", order = 1)]
public class S13ObjectSequence : S13ObjectCompound<S13ObjectSequence, S13HandlerSequence>, IEnumerable<S13Object>, IEnumerable
{
	[Serializable]
	private class ObjectFloatPair : IS13HorizontalPropertyHolder
	{
		[Tooltip("Desired delay. See DelayUnit")]
		public float delay;

		public S13Object obj;
	}

	public enum DelayUnit
	{
		Seconds = 1,
		FramesAt24FPS = 24,
		FramesAt30FPS = 30,
		FramesAt60FPS = 60
	}

	[SerializeField]
	private ObjectFloatPair[] ContainedObjects = new ObjectFloatPair[0];

	[Tooltip("This functions as a divisor, so FramesAt24FPS divides each pair by 24")]
	public DelayUnit delayUnit = DelayUnit.FramesAt30FPS;

	public IEnumerable<(S13Object obj, float delay)> GetEntries()
	{
		return this.ContainedObjects?.Select((ObjectFloatPair x) => (obj: x.obj, delay: x.delay));
	}

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.ContainedObjects?.Select((ObjectFloatPair x) => x.obj);
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		return this.ContainedObjects.Select((ObjectFloatPair x) => x.obj).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}
