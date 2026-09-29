using System;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : JMonoBehaviour
{
	[SerializeField]
	private BoxCollider m_Bounds;

	[Header("Prefabs")]
	[SerializeField]
	private Dummy[] m_DummyPrefabs;

	private CharacterControllable m_BeastBendy;

	private float m_SpawnTimer;

	private float m_SpawnTimerLimit = 2f;

	private int m_SpawnCount;

	private int m_SpawnLimit = 3;

	private List<Dummy> m_SpawnedDummies = new List<Dummy>();

	public override void Start()
	{
		if (GameManager.Instance.DummyManager == null)
		{
			GameManager.Instance.DummyManager = DummyManager.Create();
		}
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_BeastBendy == null)
		{
			this.m_BeastBendy = GameManager.Instance.BeastBendy;
		}
		if (this.m_BeastBendy == null)
		{
			return;
		}
		unchecked
		{
			for (int num = this.m_SpawnedDummies.Count - 1; num >= 0; num--)
			{
				Dummy dummy;
				dummy = this.m_SpawnedDummies[num];
				if (Vector3.Distance(dummy.transform.position, this.m_BeastBendy.transform.position) > 130f && this.m_SpawnedDummies.Contains(dummy))
				{
					this.m_SpawnedDummies.Remove(dummy);
					this.m_SpawnCount--;
					if (this.m_SpawnCount < 0)
					{
						this.m_SpawnCount = 0;
					}
					dummy.OnDeath -= this.HandleDummyOnDeath;
					dummy.ForceKil();
				}
			}
			this.m_SpawnTimer += Time.deltaTime;
			if (this.m_SpawnTimer >= this.m_SpawnTimerLimit && this.m_SpawnCount < this.m_SpawnLimit && this.m_Bounds.bounds.Contains(this.m_BeastBendy.transform.position))
			{
				this.m_SpawnTimer = 0f;
				this.Spawn();
			}
		}
	}

	public void Spawn()
	{
		unchecked
		{
			if (GameManager.Instance.DummyManager.CanSpawn)
			{
				int num;
				num = global::UnityEngine.Random.Range(0, this.m_DummyPrefabs.Length);
				Dummy dummy;
				dummy = global::UnityEngine.Object.Instantiate(this.m_DummyPrefabs[num]);
				dummy.transform.position = base.transform.position;
				dummy.transform.eulerAngles = base.transform.eulerAngles;
				dummy.OnDeath += this.HandleDummyOnDeath;
				if (!this.m_SpawnedDummies.Contains(dummy))
				{
					this.m_SpawnedDummies.Add(dummy);
				}
				this.m_SpawnCount++;
				GameManager.Instance.DummyManager.AddDummy(dummy);
			}
		}
	}

	private void HandleDummyOnDeath(object sender, EventArgs e)
	{
		Dummy dummy;
		dummy = sender as Dummy;
		dummy.OnDeath -= this.HandleDummyOnDeath;
		if (this.m_SpawnedDummies.Contains(dummy))
		{
			this.m_SpawnedDummies.Remove(dummy);
		}
		unchecked
		{
			this.m_SpawnCount--;
			if (this.m_SpawnCount < 0)
			{
				this.m_SpawnCount = 0;
			}
		}
	}
}
