using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Spawner", order = 4)]
public class S13ObjectPrimitiveSpawner : S13ObjectPrimitiveMultiClip<S13ObjectPrimitiveSpawner, S13HandlerPrimitiveSpawner>, IEnumerable<AudioClip>, IEnumerable
{
	[Space]
	public AudioClip[] spawnableClips;

	[Tooltip("The time between playing spawned instances.")]
	public S13MinMaxFloat spawnTime = new S13MinMaxFloat("Spawn Time (s)", 0f, 500f, 1f);

	[Tooltip("Should spawnTime and spawnChanceToPlay be ignored on the first spawn.")]
	public bool firstSpawnIsImmediate;

	[S13Slider("Individual Spawn Chance to Play (%)", 0f, 100f)]
	public int spawnChanceToPlay = 100;

	[Tooltip("The number of previous selections that should be avoided when randomizing the next choice. Setting to 0 will allow full randomness.")]
	public int avoidRepeatHistory;

	[S13Foldout("Advanced Settings", "S13ObjectPrimitiveSpawner")]
	[Tooltip("How should the spawned children of this behavior respond to the spawner being stopped")]
	public S13SpawnerChildrenStopBehavior stopBehavior;

	protected override void OnValidate()
	{
		if (this.spawnableClips != null)
		{
			this.avoidRepeatHistory = Mathf.Max(0, Mathf.Min(this.avoidRepeatHistory, unchecked(this.spawnableClips.Count((AudioClip x) => x) - 1)));
		}
	}

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (AudioClip item in this.spawnableClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (AudioClip item in this.spawnableClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}
}
