using System;
using UnityEngine;

public class ButcherGangController : JMonoBehaviour
{
	[Header("Active Status")]
	[SerializeField]
	private bool m_AlwaysActive;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Spawners")]
	[SerializeField]
	private ButcherGangSpawner[] m_Spawners;

	private ButcherGangSpawner m_ActiveSpawner;

	public bool AlwaysActive => this.m_AlwaysActive;

	public ButcherGangSpawner ActiveSpawner => this.m_ActiveSpawner;

	public bool IsActive { get; private set; }

	public event EventHandler OnReset;

	public void Initialize()
	{
		this.Disable();
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	public void Enable()
	{
		if (this.m_Spawners.Length == 0)
		{
			return;
		}
		ButcherGangSpawner closest;
		closest = this.GetClosest();
		if (closest != null && closest != this.m_ActiveSpawner)
		{
			if (this.AlwaysActive && this.m_ActiveSpawner != null)
			{
				this.m_ActiveSpawner.OnReturned -= this.HandleSpawnerOnReturned;
				this.m_ActiveSpawner.SetActive(active: false);
			}
			this.m_ActiveSpawner = closest;
			this.m_ActiveSpawner.OnSpawned -= this.HandleSpawnerOnSpawned;
			this.m_ActiveSpawner.OnSpawned += this.HandleSpawnerOnSpawned;
			this.m_ActiveSpawner.OnReturned -= this.HandleSpawnerOnReturned;
			this.m_ActiveSpawner.OnReturned += this.HandleSpawnerOnReturned;
			this.m_ActiveSpawner.SetActive(active: true);
		}
	}

	private void HandleSpawnerOnSpawned(object sender, EventArgs e)
	{
		this.m_ActiveSpawner.OnSpawned -= this.HandleSpawnerOnSpawned;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.ButcherGang);
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
	}

	public void Disable()
	{
		for (int i = 0; i < this.m_Spawners.Length; i = unchecked(i + 1))
		{
			this.m_Spawners[i].SetActive(active: false);
		}
	}

	public void ResetActiveSpawner()
	{
		if ((bool)this.m_ActiveSpawner)
		{
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
			}
			this.m_ActiveSpawner.OnSpawned -= this.HandleSpawnerOnSpawned;
			this.m_ActiveSpawner.OnReturned -= this.HandleSpawnerOnReturned;
			this.m_ActiveSpawner = null;
		}
	}

	private void HandleSpawnerOnReturned(object sender, EventArgs e)
	{
		this.m_ActiveSpawner.OnReturned -= this.HandleSpawnerOnReturned;
		this.m_ActiveSpawner.OnSpawned -= this.HandleSpawnerOnSpawned;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.ButcherGang)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
		this.m_ActiveSpawner.SetActive(active: false);
		this.OnReset.Send(this);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Dead);
		for (int i = 0; i < this.m_Spawners.Length; i = unchecked(i + 1))
		{
			this.m_Spawners[i].Cancel();
		}
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.IsActive = true;
		}
		return flag;
	}

	private ButcherGangSpawner GetClosest()
	{
		ButcherGangSpawner result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		if (this.m_Spawners.Length == 1)
		{
			this.ResetActiveSpawner();
		}
		ButcherGangSpawner[] spawners;
		spawners = this.m_Spawners;
		foreach (ButcherGangSpawner butcherGangSpawner in spawners)
		{
			if (this.AlwaysActive || !(butcherGangSpawner == this.m_ActiveSpawner))
			{
				float num2;
				num2 = Vector3.Distance(butcherGangSpawner.transform.position, position);
				if (num2 < num)
				{
					result = butcherGangSpawner;
					num = num2;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		this.ResetActiveSpawner();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
