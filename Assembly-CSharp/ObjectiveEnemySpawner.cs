using System;
using UnityEngine;
using UnityEngine.AI;

public class ObjectiveEnemySpawner : Objective
{
	[Serializable]
	public class EnemySpawnerGroup
	{
		public Enemy Prefab;

		public Transform Location;

		public bool TargetPlayer;
	}

	[SerializeField]
	private EnemySpawnerGroup[] m_EnemySpawnerGroup;

	protected override void InternalInitialize()
	{
		if (this.m_EnemySpawnerGroup != null && this.m_EnemySpawnerGroup.Length != 0)
		{
			for (int i = 0; i < this.m_EnemySpawnerGroup.Length; i = unchecked(i + 1))
			{
				EnemySpawnerGroup enemySpawnerGroup;
				enemySpawnerGroup = this.m_EnemySpawnerGroup[i];
				if (enemySpawnerGroup.Prefab != null && enemySpawnerGroup.Location != null)
				{
					Enemy enemy;
					enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(enemySpawnerGroup.Prefab);
					enemy.SetSection(base.GetComponentInParent<Section>().SectionID);
					enemy.transform.position = enemySpawnerGroup.Location.position;
					enemy.transform.eulerAngles = enemySpawnerGroup.Location.eulerAngles;
					if (enemySpawnerGroup.TargetPlayer)
					{
						enemy.OnInitializeOnComplete -= this.HandleEnemyOnINitializeOnComplete;
						enemy.OnInitializeOnComplete += this.HandleEnemyOnINitializeOnComplete;
					}
				}
			}
		}
		base.SendOnComplete();
	}

	private void HandleEnemyOnINitializeOnComplete(object sender, EventArgs e)
	{
		Enemy obj;
		obj = sender as Enemy;
		obj.OnInitializeOnComplete -= this.HandleEnemyOnINitializeOnComplete;
		obj.SetTarget(GameManager.Instance.Player.transform);
		obj.SetState(State.Character.Follow);
		obj.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		obj.Agent.ResetAgent();
	}
}
