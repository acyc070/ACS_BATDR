using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionButcherGangController : SectionController
{
	[Header("Timer")]
	[SerializeField]
	private float m_TimerLimitMin = 180f;

	[SerializeField]
	private float m_TimerLimitMax = 360f;

	[SerializeField]
	private float m_ActiveTimerLimit = 60f;

	private ButcherGangController[] m_Group;

	private ButcherGangController m_CurrentButcherGangController;

	private bool IsButcherGangActive;

	private float m_Timer;

	private float m_TimerLimit;

	private float m_ActiveTimer;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<ButcherGangController>(includeInactive: true);
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].Initialize();
			}
			this.AddListeners();
		}
		yield return null;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || this.m_Group == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.IsButcherGangActive)
		{
			if (this.m_ActiveTimer >= this.m_ActiveTimerLimit && this.m_CurrentButcherGangController.ActiveSpawner != null && !this.m_CurrentButcherGangController.ActiveSpawner.IsSpawned)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					if (!this.m_CurrentButcherGangController.AlwaysActive)
					{
						this.m_CurrentButcherGangController.Disable();
					}
					this.m_ActiveTimer = 0f;
					this.ActivateButcherGang();
				}
			}
			else
			{
				this.m_ActiveTimer += Time.deltaTime;
			}
		}
		else
		{
			if (!this.IsAvailable())
			{
				return;
			}
			if (this.m_Timer >= this.m_TimerLimit)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					this.IsButcherGangActive = true;
					this.ActivateButcherGang();
				}
			}
			else
			{
				this.m_Timer += Time.deltaTime;
			}
		}
	}

	private bool IsAvailable()
	{
		bool result;
		result = false;
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				if (this.m_Group[i].IsActive)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private void ActivateButcherGang()
	{
		if (this.m_Group.Length > 1)
		{
			this.m_Group.Shuffle();
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				ButcherGangController butcherGangController;
				butcherGangController = this.m_Group[i];
				if (butcherGangController.IsActive)
				{
					this.m_CurrentButcherGangController = butcherGangController;
					this.m_CurrentButcherGangController.Enable();
					break;
				}
			}
		}
		else if (this.m_Group != null && this.m_Group.Length != 0)
		{
			this.m_CurrentButcherGangController = this.m_Group[0];
			this.m_CurrentButcherGangController.Enable();
		}
	}

	private void ResetTimeLimit()
	{
		this.m_Timer = 0f;
		this.m_TimerLimit = global::UnityEngine.Random.Range(this.m_TimerLimitMin, this.m_TimerLimitMax);
	}

	private void HandleGroupOnReset(object sender, EventArgs e)
	{
		this.ResetTimeLimit();
		this.IsButcherGangActive = false;
	}

	private void AddListeners()
	{
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				ButcherGangController obj;
				obj = this.m_Group[i];
				obj.OnReset -= this.HandleGroupOnReset;
				obj.OnReset += this.HandleGroupOnReset;
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].OnReset -= this.HandleGroupOnReset;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
