using System;
using UnityEngine;

public class EventTriggerDamage : EventTrigger
{
	[Header("Hit Damage")]
	[SerializeField]
	private int m_HitDamage = 1;

	[SerializeField]
	private bool m_CanKill = true;

	[SerializeField]
	private bool m_ShowDamageEffect = true;

	[Header("Hit Force")]
	[SerializeField]
	private float m_ForceAmount = 30f;

	[SerializeField]
	private float m_ForceYOffset = 2f;

	[Header("Hit Impact")]
	[SerializeField]
	private Impact m_Prefab;

	[SerializeField]
	private float m_ImpactScale = 1f;

	public event EventHandler OnDamage;

	protected override void OnInternalEnter(Collider col)
	{
		RaycastHit hit;
		hit = default;
		Vector3 vector;
		vector = (hit.point = base.transform.position);
		if (this.m_Prefab != null)
		{
			Impact impact;
			impact = GameManager.Instance.AssetManager.CreateAsset<Impact>(this.m_Prefab);
			impact.Initialize(hit);
			impact.Content.localScale = Vector3.one * this.m_ImpactScale;
		}
		vector.y = GameManager.Instance.Player.transform.position.y - this.m_ForceYOffset;
		Vector3 vector2;
		vector2 = GameManager.Instance.Player.transform.position - vector;
		hit.point = vector;
		int damage;
		damage = this.m_HitDamage;
		if (!this.m_CanKill && GameManager.Instance.Player.Health <= (float)this.m_HitDamage)
		{
			damage = 0;
		}
		if (this.m_ShowDamageEffect)
		{
			GameManager.Instance.Player.Hit(hit, null, damage);
		}
		GameManager.Instance.Player.AddForce(vector2.normalized * this.m_ForceAmount);
		CameraEffects.ShakeRotation(1f, 2f);
		base.ResetAction();
		this.OnDamage.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnDamage = null;
		base.OnDisposed();
	}
}
