using UnityEngine;

public class EnemyRandomSequence : JMonoBehaviour
{
	[SerializeField]
	private EnemySelector m_EnemySelector;

	[SerializeField]
	private Transform[] m_SpawnPoints;

	[SerializeField]
	private Collider m_Bounds;

	private float m_Timer;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Bounds.bounds.Contains(GameManager.Instance.Player.transform.position))
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide && GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth)
			{
				this.m_Timer += Time.deltaTime;
				if (this.m_Timer > 10f)
				{
					Enemy enemy;
					enemy = this.m_EnemySelector.Get();
					enemy.transform.position = this.m_SpawnPoints[Random.Range(0, this.m_SpawnPoints.Length)].position;
					enemy.Initialize();
					enemy.SetTarget(GameManager.Instance.Player.transform);
					this.m_Timer = 0f;
				}
			}
		}
		else
		{
			this.m_Timer = 0f;
		}
	}
}
