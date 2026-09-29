using System;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : JMonoBehaviour
{
	[SerializeField]
	private Companion m_CompanionPrefab;

	private List<SectionZone> m_ActiveZones = new List<SectionZone>();

	private SectionZone m_LastKnownZone;

	private float m_TimeInZone;

	private Companion m_Companion;

	private Transform m_CurrentTarget;

	private Transform m_PreviousTarget;

	private bool hasInkDemon;

	private Player m_Player => GameManager.Instance.Player;

	public SectionZone CurrentZone { get; private set; }

	public static AiManager Create()
	{
		AiManager aiManager;
		aiManager = new GameObject("[AI MANAGER]").AddComponent<AiManager>();
		aiManager.Initialize();
		return aiManager;
	}

	public void Initialize()
	{
	}

	private void Update()
	{
		if (!this.hasInkDemon)
		{
			this.UpdateCompanion();
		}
	}

	private void UpdateInkDemon()
	{
	}

	private void UpdateCompanion()
	{
		if (this.CurrentZone == null)
		{
			return;
		}
		this.m_TimeInZone += Time.deltaTime;
		if (this.m_Companion != null)
		{
			this.m_Companion.Timer = this.m_TimeInZone;
		}
		if (!(this.m_TimeInZone > 5f))
		{
			return;
		}
		this.m_TimeInZone = 0f;
		if (this.m_Companion != null)
		{
			Vector3 position;
			position = this.m_Companion.transform.position;
			float num;
			num = Vector3.Distance(GameManager.Instance.Player.transform.position, position);
			if (GameManager.Instance.GameCamera.CheckPositionAngle(position) && num > 50f)
			{
				this.hasInkDemon = true;
				this.UpdateInkDemon();
				this.m_Companion.Dispose();
				return;
			}
		}
		if (this.m_Companion == null)
		{
			for (int i = 0; i < this.m_ActiveZones.Count; i = unchecked(i + 1))
			{
				if (this.m_ActiveZones[i] != this.CurrentZone)
				{
					Vector3 position2;
					position2 = this.m_ActiveZones[i].transform.position;
					if (GameManager.Instance.GameCamera.CheckPositionAngle(position2, 25f))
					{
						this.m_Companion = global::UnityEngine.Object.Instantiate(this.m_CompanionPrefab);
						this.m_Companion.transform.position = position2;
					}
					break;
				}
			}
		}
		_ = this.m_Companion != null;
	}

	private void HandleCompanionAnimationOnComplete(object sender, EventArgs e)
	{
		this.m_Companion.OnSpecialAnimationComplete -= this.HandleCompanionAnimationOnComplete;
		this.m_Companion.SetNode(this.m_CurrentTarget.GetComponent<MonoBehaviour>() as CompanionNode);
	}

	private void HandleZoneOnEnter(object sender, EventArgs e)
	{
		this.CurrentZone = sender as SectionZone;
		this.m_LastKnownZone = this.CurrentZone;
		this.m_TimeInZone = 0f;
	}

	private void HandleZoneOnExit(object sender, EventArgs e)
	{
		this.CurrentZone = null;
		this.m_TimeInZone = 0f;
	}

	public void AddZones(SectionZone[] zones)
	{
		if (zones == null)
		{
			return;
		}
		for (int i = 0; i < zones.Length; i = unchecked(i + 1))
		{
			if (!this.m_ActiveZones.Contains(zones[i]))
			{
				this.m_ActiveZones.Add(zones[i]);
				zones[i].OnEnter += this.HandleZoneOnEnter;
				zones[i].OnExit += this.HandleZoneOnExit;
			}
		}
	}

	public void RemoveZones(SectionZone[] zones)
	{
		if (zones == null)
		{
			return;
		}
		unchecked
		{
			for (int num = zones.Length - 1; num > -1; num--)
			{
				if (this.m_ActiveZones.Contains(zones[num]))
				{
					this.m_ActiveZones.Remove(zones[num]);
					zones[num].OnEnter -= this.HandleZoneOnEnter;
					zones[num].OnExit -= this.HandleZoneOnExit;
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		this.m_ActiveZones?.Clear();
		this.m_ActiveZones = null;
		this.CurrentZone = null;
		this.m_LastKnownZone = null;
		base.OnDisposed();
	}
}
