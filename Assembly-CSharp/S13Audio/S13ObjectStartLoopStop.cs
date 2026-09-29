using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Start-Loop-Stop", order = 3)]
public class S13ObjectStartLoopStop : S13ObjectCompound<S13ObjectStartLoopStop, S13HandlerStartLoopStop>, IEnumerable<S13Object>, IEnumerable
{
	public S13Object startSegment;

	public S13Object loopSegment;

	public S13Object stopSegment;

	[Tooltip("If the start segment is playing, should it be cut off when the object is stopped?")]
	public bool cutStart;

	[Tooltip("If the stop segment is playing, should it be cut off when the object is played? This always respects fades")]
	public bool cutStop;

	[Tooltip("How long after Start plays should the Loop segment wait")]
	public float loopDelay;

	public override IEnumerable<S13Object> GetDependencies()
	{
		yield return this.startSegment;
		yield return this.loopSegment;
		yield return this.stopSegment;
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		if ((bool)this.startSegment)
		{
			yield return this.startSegment;
		}
		if ((bool)this.loopSegment)
		{
			yield return this.loopSegment;
		}
		if ((bool)this.stopSegment)
		{
			yield return this.stopSegment;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if ((bool)this.startSegment)
		{
			yield return this.startSegment;
		}
		if ((bool)this.loopSegment)
		{
			yield return this.loopSegment;
		}
		if ((bool)this.stopSegment)
		{
			yield return this.stopSegment;
		}
	}
}
