using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerSelector : S13HandlerCompound<S13ObjectSelector, S13HandlerSelector>
{
	private int isPlayingSemaphore;

	private Queue<(int index, S13Handler handler)> history = new Queue<(int, S13Handler)>();

	public override bool isPlaying
	{
		get
		{
			if (this.isPlayingSemaphore <= 0)
			{
				return base.isPlaying;
			}
			return true;
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		base.StartCoroutine(this.PlayRoutine(parent));
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		base.StopAllCoroutines();
		this.isPlayingSemaphore = 0;
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
		base.ResumeHandler();
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Resume();
		}
	}

	private IEnumerator PlayRoutine(Transform parent)
	{
		unchecked
		{
			this.isPlayingSemaphore++;
			float waitTime;
			waitTime = base.targetObject.startDelay;
			while (waitTime > 0f)
			{
				yield return null;
				if (!base.isPaused)
				{
					waitTime -= base.deltaTime;
				}
			}
			this.isPlayingSemaphore--;
			int num;
			num = -1;
			S13WeightedObjectList.S13WeightedObjectEntry[] weightedObjects;
			weightedObjects = base.targetObject.selectableWeightedObjects.GetWeightedObjects();
			List<int> list;
			list = new List<int>(weightedObjects.Length - this.history.Count);
			float num2;
			num2 = 0f;
			int i;
			for (i = 0; i < weightedObjects.Length; i++)
			{
				if (this.history.All(((int index, S13Handler handler) x) => x.index != i))
				{
					num2 += Mathf.Max(weightedObjects[i].weight, float.Epsilon);
					list.Add(i);
				}
			}
			float num3;
			num3 = Random.Range(0f, num2);
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				if ((num3 -= weightedObjects[list[num4]].weight) < 0f)
				{
					num = list[num4];
					break;
				}
			}
			if (num >= 0)
			{
				S13Handler s13Handler;
				s13Handler = base.subHandlers[num];
				s13Handler.Play(parent);
				this.history.Enqueue((num, s13Handler));
				while (this.history.Count > 0 && this.history.Count > base.targetObject.avoidRepeatHistory)
				{
					this.history.Dequeue();
				}
			}
		}
	}
}
