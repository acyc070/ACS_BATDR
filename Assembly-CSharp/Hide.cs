using UnityEngine;

public class Hide : EventTrigger
{
	private Transform m_ProtectiveCollider;

	private void Update()
	{
		if (this.m_ProtectiveCollider == null || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		this.m_ProtectiveCollider.position = GameManager.Instance.Player.transform.position;
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			return;
		}
		Object.Destroy(this.m_ProtectiveCollider.gameObject);
		this.m_ProtectiveCollider = null;
		if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat)
		{
			if (GameManager.Instance.Player.IsCrouched)
			{
				GameManager.Instance.Player.SetCombatStatus(CombatStatus.Stealth);
			}
			else
			{
				GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
			}
		}
	}

	protected override void OnInternalEnter(Collider col)
	{
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.ClearEnemies();
		this.GenerateProtectiveCollider();
	}

	private void GenerateProtectiveCollider()
	{
		if (this.m_ProtectiveCollider == null)
		{
			this.m_ProtectiveCollider = new GameObject("[Protective Collider]").transform;
		}
		this.m_ProtectiveCollider.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
		this.m_ProtectiveCollider.SetParent(base.transform);
		this.m_ProtectiveCollider.position = GameManager.Instance.Player.transform.position;
		CapsuleCollider capsuleCollider;
		capsuleCollider = this.m_ProtectiveCollider.gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.center = new Vector3(0f, 3.5f, 0f);
		capsuleCollider.radius = 1.5f;
		capsuleCollider.height = 7f;
	}

	protected override void OnInternalExit(Collider col)
	{
		if (GameManager.Instance.Player.IsCrouched)
		{
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.Stealth);
		}
		else
		{
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
		}
		if (this.m_ProtectiveCollider != null)
		{
			Object.Destroy(this.m_ProtectiveCollider.gameObject);
			this.m_ProtectiveCollider = null;
		}
	}
}
