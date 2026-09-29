using UnityEngine;

public class ObjectiveSpawners : Objective
{
	[SerializeField]
	private bool m_InstantSpawn;

	[SerializeField]
	private Spawner[] m_Spawners;

	protected override void InternalInitialize()
	{
		this.Activate();
	}

	protected override void InternalEnable()
	{
		this.Activate();
	}

	private void Activate()
	{
		for (int i = 0; i < this.m_Spawners.Length; i = unchecked(i + 1))
		{
			if (this.m_InstantSpawn)
			{
				this.m_Spawners[i].Spawn();
			}
			else
			{
				this.m_Spawners[i].Activate();
			}
		}
	}

	protected override void InternalDisable()
	{
		this.Deactivate();
	}

	protected override void InternalInactive()
	{
		this.Deactivate();
	}

	private void Deactivate()
	{
		for (int i = 0; i < this.m_Spawners.Length; i = unchecked(i + 1))
		{
			this.m_Spawners[i].Deactivate();
		}
	}
}
