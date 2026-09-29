using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerSpawner : S13HandlerCompound<S13ObjectSpawner, S13HandlerSpawner>
{
	private Coroutine spawner;

	private Transform targetParent;

	private Queue<(int index, S13Handler handler)> history = new Queue<(int, S13Handler)>();

	public override bool isPlaying
	{
		get
		{
			if (this.spawner == null)
			{
				return base.isPlaying;
			}
			return true;
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		this.targetParent = parent;
		if (!this.isPlaying)
		{
			this.spawner = base.StartCoroutine(this.DoSpawning());
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		switch (base.targetObject.stopBehavior)
		{
		case S13SpawnerChildrenStopBehavior.Fade:
			foreach (S13Handler subHandler in base.subHandlers)
			{
				if (subHandler.isPlaying)
				{
					subHandler.Stop(ignoreFades);
				}
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case S13SpawnerChildrenStopBehavior.Continue:
			break;
		}
		if (this.spawner != null)
		{
			base.StopCoroutine(this.spawner);
		}
		this.spawner = null;
	}

	private IEnumerator DoSpawning()
	{
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
		bool firstSpawnOverride;
		firstSpawnOverride = base.targetObject.firstSpawnIsImmediate;
		unchecked
		{
			while (true)
			{
				waitTime = base.targetObject.spawnTime.value;
				while (waitTime > 0f && !firstSpawnOverride)
				{
					yield return null;
					if (!base.isPaused)
					{
						waitTime -= base.deltaTime;
					}
				}
				int num;
				num = -1;
				S13Object[] spawnableObjects;
				spawnableObjects = base.targetObject.spawnableObjects;
				List<int> list;
				list = new List<int>(spawnableObjects.Length - this.history.Count);
				int i;
				for (i = 0; i < spawnableObjects.Length; i++)
				{
					if (this.history.All(((int index, S13Handler handler) x) => x.index != i))
					{
						list.Add(i);
					}
				}
				int num2;
				num2 = global::UnityEngine.Random.Range(0, list.Count);
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					if (--num2 < 0)
					{
						num = list[num3];
						break;
					}
				}
				if (num < 0 || num >= base.subHandlers.Count)
				{
					continue;
				}
				S13Handler s13Handler;
				s13Handler = base.subHandlers[num];
				if (base.targetObject.spawnChanceToPlay >= 100 || firstSpawnOverride || !(global::UnityEngine.Random.Range(0f, 100f) >= (float)base.targetObject.spawnChanceToPlay))
				{
					firstSpawnOverride = false;
					s13Handler.Play(this.targetParent);
					this.history.Enqueue((num, s13Handler));
					while (this.history.Count > 0 && this.history.Count > base.targetObject.avoidRepeatHistory)
					{
						this.history.Dequeue();
					}
				}
			}
		}
	}
}
