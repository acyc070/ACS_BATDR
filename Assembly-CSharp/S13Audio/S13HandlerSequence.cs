using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerSequence : S13HandlerCompound<S13ObjectSequence, S13HandlerSequence>
{
	private Dictionary<S13Handler, float> delayDictionary = new Dictionary<S13Handler, float>();

	private List<(S13Object obj, float delay)> _unmatchedQueue = new List<(S13Object, float)>();

	public override void Initialize(S13ObjectSequence targetObject)
	{
		base.Initialize(targetObject);
		this._unmatchedQueue = targetObject.GetEntries().ToList();
	}

	public override void RegisterSubHandler(S13Handler subHandler)
	{
		base.RegisterSubHandler(subHandler);
		(S13Object, float) tuple;
		tuple = this._unmatchedQueue.FirstOrDefault(((S13Object obj, float delay) x) => x.obj == subHandler.baseObject);
		var (s13Object, num) = tuple;
		if (s13Object != null || num != 0f)
		{
			this._unmatchedQueue.Remove(tuple);
			this.delayDictionary.Add(subHandler, tuple.Item2);
		}
		else
		{
			S13Debug.LogWarning("Could not Register SubHandler " + subHandler.name + " on S13HandlerSequencer " + base.name, this);
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.PlayDelayed(parent, this.delayDictionary[subHandler] / (float)Mathf.Max(1, unchecked((int)base.targetObject.delayUnit)));
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Stop(ignoreFades);
		}
	}

	protected override void PauseHandler()
	{
		base.PauseHandler();
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Pause();
		}
	}

	protected override void ResumeHandler()
	{
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Resume();
		}
	}
}
