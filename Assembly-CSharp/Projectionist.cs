using System;
using UnityEngine;

public class Projectionist : JMonoBehaviour, IHittable
{
	[SerializeField]
	private GameObject m_Lights;

	private bool m_IsHit;

	private float m_Timer;

	private float m_TimerRate = 10f;

	public bool IsPlayerBreakable => true;

	public bool IsBroken => false;

	public event EventHandler OnHit;

	public event EventHandler OnOff;

	public override void Start()
	{
		this.m_Lights.SetActive(value: false);
		this.m_Timer = this.m_TimerRate;
		this.m_IsHit = false;
	}

	private void Update()
	{
		if (this.m_IsHit && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (this.m_Timer >= this.m_TimerRate)
			{
				this.m_IsHit = false;
				this.m_Timer = 0f;
				this.m_Lights.SetActive(value: false);
				this.OnOff.Send(this);
			}
			else
			{
				this.m_Timer += Time.deltaTime;
			}
		}
	}

	public void Hit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Sparks", 8f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		if (!this.m_IsHit)
		{
			this.m_IsHit = true;
			this.m_Lights.SetActive(value: true);
			this.OnHit.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.OnOff = null;
		base.OnDisposed();
	}
}
