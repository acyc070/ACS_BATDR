using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPrimitiveSpawner : S13HandlerPrimitiveMultiClip<S13ObjectPrimitiveSpawner, S13HandlerPrimitiveSpawner>
{
	private Coroutine spawner;

	private Transform targetParent;

	private Queue<(int index, AudioClip clip)> history = new Queue<(int, AudioClip)>();

	public override bool isPlaying
	{
		get
		{
			if (!base.isPlaying)
			{
				return this.spawner != null;
			}
			return true;
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		if (this.isPlaying)
		{
			switch (base.targetObject.voiceMode)
			{
			case S13Object.VoiceMode.KillOld:
				this.StopHandler(ignoreFades: true);
				break;
			case S13Object.VoiceMode.IgnoreNew:
				return;
			case S13Object.VoiceMode.UseClosest:
			{
				Vector3 a;
				a = S13Manager.GetAudioListener()?.transform?.position ?? Vector3.zero;
				float num;
				num = Vector3.Distance(a, this.targetParent.position);
				if (Vector3.Distance(a, parent.transform.position) > num)
				{
					return;
				}
				this.StopHandler(ignoreFades: true);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		this.targetParent = parent;
		this.spawner = base.StartCoroutine(this.DoSpawning());
	}

	protected override void StopHandler(bool ignoreFades)
	{
		switch (base.targetObject.stopBehavior)
		{
		case S13SpawnerChildrenStopBehavior.Fade:
			base.StopHandler(ignoreFades);
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
				AudioClip[] spawnableClips;
				spawnableClips = base.targetObject.spawnableClips;
				List<int> list;
				list = new List<int>(spawnableClips.Length - this.history.Count);
				int i;
				for (i = 0; i < spawnableClips.Length; i++)
				{
					if (this.history.All(((int index, AudioClip clip) x) => x.index != i))
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
				if (num < 0 || num >= spawnableClips.Length)
				{
					continue;
				}
				AudioClip audioClip;
				audioClip = spawnableClips[num];
				if (base.targetObject.spawnChanceToPlay >= 100 || firstSpawnOverride || !(global::UnityEngine.Random.Range(0f, 100f) >= (float)base.targetObject.spawnChanceToPlay))
				{
					firstSpawnOverride = false;
					base.VoiceClip(audioClip, base.targetObject.loop, this.targetParent);
					this.history.Enqueue((num, audioClip));
					while (this.history.Count > 0 && this.history.Count > base.targetObject.avoidRepeatHistory)
					{
						this.history.Dequeue();
					}
				}
			}
		}
	}
}
