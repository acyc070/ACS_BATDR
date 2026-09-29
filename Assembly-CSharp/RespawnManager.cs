using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : JDisposable
{
	private List<PlayerRespawn> m_Respawner = new List<PlayerRespawn>();

	public static RespawnManager Create()
	{
		return new RespawnManager();
	}

	public PlayerRespawn GetClosest(Vector3 _currentPosition)
	{
		PlayerRespawn result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		foreach (PlayerRespawn item in this.m_Respawner)
		{
			if (item == null || !item.IsActive)
			{
				continue;
			}
			Section componentInParent;
			componentInParent = item.GetComponentInParent<Section>(includeInactive: true);
			if (!(componentInParent == null) && componentInParent.IsActive && componentInParent.IsInitialized)
			{
				float num2;
				num2 = Vector3.Distance(item.transform.position, _currentPosition);
				if (num2 < num)
				{
					result = item;
					num = num2;
				}
			}
		}
		return result;
	}

	public PlayerRespawn GetRespawner(PlayerRespawn respawner)
	{
		PlayerRespawn result;
		result = null;
		if (this.Contains(respawner))
		{
			result = this.m_Respawner[this.m_Respawner.IndexOf(respawner)];
		}
		return result;
	}

	public bool Contains(PlayerRespawn respawner)
	{
		return this.m_Respawner.Contains(respawner);
	}

	public void Add(PlayerRespawn respawner)
	{
		if (!this.Contains(respawner))
		{
			this.m_Respawner.Add(respawner);
		}
	}

	public void Remove(PlayerRespawn respawner)
	{
		if (this.Contains(respawner))
		{
			this.m_Respawner.Remove(respawner);
		}
	}

	public void Clear()
	{
		if (this.m_Respawner != null)
		{
			this.m_Respawner.Clear();
		}
	}

	protected override void OnDisposed()
	{
		this.Clear();
		this.m_Respawner = null;
		base.OnDisposed();
	}
}
