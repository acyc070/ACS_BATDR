using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Spawner", order = 2)]
public class S13ObjectSpawner : S13ObjectCompound<S13ObjectSpawner, S13HandlerSpawner>, IEnumerable<S13Object>, IEnumerable
{
	public S13Object[] spawnableObjects;

	[Tooltip("The time between playing spawned instances.")]
	public S13MinMaxFloat spawnTime = new S13MinMaxFloat("Spawn Time (s)", 0f, 30f, 1f);

	[Tooltip("Should the first spawn be immediate or use spawnTime")]
	public bool firstSpawnIsImmediate;

	[S13Slider("Individual Spawn Chance to Play (%)", 0f, 100f)]
	public int spawnChanceToPlay = 100;

	[Tooltip("The number of previous selections that should be avoided when randomizing the next choice. Setting to 0 will allow full randomness.")]
	public int avoidRepeatHistory;

	[S13Foldout("Advanced Settings", "S13ObjectObjectSpawner")]
	[Tooltip("How should the spawned children of this behavior respond to the spawner being stopped")]
	public S13SpawnerChildrenStopBehavior stopBehavior;

	private void OnValidate()
	{
		this.avoidRepeatHistory = Mathf.Max(0, Mathf.Min(this.avoidRepeatHistory, unchecked(this.spawnableObjects.Count((S13Object x) => x) - 1)));
	}

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.spawnableObjects;
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.spawnableObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13Object item in this.spawnableObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}
}
