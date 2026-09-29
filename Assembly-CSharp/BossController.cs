using System;
using S13Audio;
using UnityEngine;

public class BossController : JMonoBehaviour
{
	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_BossCutscene;

	[Header("ShipAhoy")]
	[SerializeField]
	private Transform m_ShipAhoy;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private GameObject m_ShipAhoySparks;

	[SerializeField]
	private HittableCharacter m_HittableCharacter;

	[Header("Phases")]
	[SerializeField]
	private BossPhaseController m_PhaseController;

	[SerializeField]
	private BossPhase m_PhaseOne;

	[SerializeField]
	private BossPhase m_PhaseWave;

	[SerializeField]
	private BossPhase m_PhaseTwo;

	[Header("Chargers")]
	[SerializeField]
	private GentRechargerInstant m_GentRechargerInstant;

	[Header("Towers")]
	[SerializeField]
	private GameObject m_CenterLights;

	[SerializeField]
	private GentPower[] m_GentPowers;

	[SerializeField]
	private Transform[] m_EdisonTowers;

	[SerializeField]
	private SignalTower[] m_SignalTowers;

	[SerializeField]
	private S13ScriptableEvent[] m_S13ScriptableEvents;

	private int m_TowerCount;

	public override void Awake()
	{
		this.m_ShipAhoy.gameObject.SetActive(value: false);
		this.m_CenterLights.SetActive(value: false);
		unchecked
		{
			for (int i = 0; i < this.m_EdisonTowers.Length; i++)
			{
				this.m_EdisonTowers[i].gameObject.SetActive(value: false);
			}
			for (int j = 0; j < this.m_SignalTowers.Length; j++)
			{
				this.m_SignalTowers[j].TurnOff();
			}
			for (int k = 0; k < this.m_GentPowers.Length; k++)
			{
				this.m_GentPowers[k].OnComplete -= this.HandleActionEventControllerOnInteractionComplete;
				this.m_GentPowers[k].OnComplete += this.HandleActionEventControllerOnInteractionComplete;
			}
			this.m_ShipAhoySparks.SetActive(value: false);
			this.m_BossCutscene.OnComplete -= this.HandleBossOnComplete;
			this.m_BossCutscene.OnComplete += this.HandleBossOnComplete;
			this.m_PhaseWave.OnEnd -= this.HandlePhaseWaveOnEnd;
			this.m_PhaseWave.OnEnd += this.HandlePhaseWaveOnEnd;
			this.m_PhaseController.OnComplete -= this.HandleBossPhaseControllerOnComplete;
			this.m_PhaseController.OnComplete += this.HandleBossPhaseControllerOnComplete;
		}
	}

	private void HandleBossPhaseControllerOnComplete(object sender, EventArgs e)
	{
		if (this.m_PhaseController != null)
		{
			this.m_PhaseController.OnComplete -= this.HandleBossPhaseControllerOnComplete;
		}
		if (this.m_BossCutscene != null)
		{
			this.m_BossCutscene.OnComplete -= this.HandleBossOnComplete;
		}
		if (this.m_PhaseWave != null)
		{
			this.m_PhaseWave.OnEnd -= this.HandlePhaseWaveOnEnd;
		}
		this.m_ShipAhoySparks.SetActive(value: false);
		this.m_HittableCharacter.SetActive(active: false);
		this.m_ShipAhoy.gameObject.SetActive(value: false);
		this.m_CenterLights.SetActive(value: false);
	}

	private void HandlePhaseWaveOnEnd(object sender, EventArgs e)
	{
		this.m_ShipAhoySparks.SetActive(value: false);
	}

	private void HandleBossOnComplete(object sender, EventArgs e)
	{
		this.m_BossCutscene.OnComplete -= this.HandleBossOnComplete;
		this.m_ShipAhoy.gameObject.SetActive(value: true);
		this.m_HittableCharacter.SetActive(active: true);
		this.m_PhaseController.Initialize();
		this.m_GentRechargerInstant.SetActive(active: true);
		this.m_CenterLights.SetActive(value: true);
		unchecked
		{
			for (int i = 0; i < this.m_EdisonTowers.Length; i++)
			{
				this.m_EdisonTowers[i].gameObject.SetActive(value: true);
			}
			for (int j = 0; j < this.m_SignalTowers.Length; j++)
			{
				this.m_SignalTowers[j].TurnOn();
			}
			if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Easy && GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Normal)
			{
				return;
			}
			for (int k = 0; k < this.m_GentPowers.Length; k++)
			{
				Sparkles componentInChildren;
				componentInChildren = this.m_GentPowers[k].GetComponentInChildren<Sparkles>();
				if (componentInChildren != null)
				{
					componentInChildren.Play();
				}
			}
		}
	}

	private void HandleActionEventControllerOnInteractionComplete(object sender, EventArgs e)
	{
		unchecked
		{
			this.m_TowerCount++;
			GentPower gentPower;
			gentPower = (GentPower)sender;
			int num;
			num = 0;
			for (int i = 0; i < this.m_GentPowers.Length; i++)
			{
				if (gentPower.GentPowerID == this.m_GentPowers[i].GentPowerID)
				{
					num = i;
					break;
				}
			}
			this.m_EdisonTowers[num].gameObject.SetActive(value: false);
			this.m_SignalTowers[num].TurnOff();
			this.m_S13ScriptableEvents[num].Raise();
			if (this.m_TowerCount == 4)
			{
				this.SetPhase(this.m_PhaseTwo);
			}
			else if (this.m_TowerCount == 2)
			{
				this.SetPhase(this.m_PhaseOne);
			}
		}
	}

	public void ActivateSignalTower(int index)
	{
		if (index == 0)
		{
			this.m_CenterLights.SetActive(value: true);
		}
		this.m_EdisonTowers[index].gameObject.SetActive(value: true);
		this.m_SignalTowers[index].TurnOn();
	}

	private void SetPhase(BossPhase phaseComplete)
	{
		phaseComplete.Complete();
		this.m_GentRechargerInstant.SetActive(active: false);
		this.m_ShipAhoySparks.SetActive(value: true);
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_GentPowers.Length; i = unchecked(i + 1))
		{
			this.m_GentPowers[i].OnComplete -= this.HandleActionEventControllerOnInteractionComplete;
		}
		this.m_BossCutscene.OnComplete -= this.HandleBossOnComplete;
		this.m_PhaseWave.OnEnd -= this.HandlePhaseWaveOnEnd;
		base.OnDisposed();
	}
}
