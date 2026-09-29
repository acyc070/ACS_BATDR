using System;
using UnityEngine;

public class InkDemonManager : JMonoBehaviour
{
	private bool m_IsApproaching;

	private float m_Timer;

	private float m_TimerLimit;

	public bool IsActive { get; private set; }

	public float Timer => this.m_Timer;

	public float TimerLimit => this.m_TimerLimit;

	public event EventHandler OnApproaching;

	public static InkDemonManager Create()
	{
		return GameManager.Instance.AssetManager.CreateAsset<InkDemonManager>("Controllers/InkDemonManager");
	}

	public override void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (!this.IsActive || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Timer >= this.m_TimerLimit)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide && GameManager.Instance.Player.BattleStatus == BattleStatus.None && !GameManager.Instance.Player.IsUnderWanter && GameManager.Instance.Player.CurrentState == State.Player.Default)
			{
				this.SetActive(active: false);
				this.ResetTimer();
				GameManager.Instance.Player.InkDemonTest();
				this.m_IsApproaching = false;
			}
			return;
		}
		if (!this.m_IsApproaching && this.m_Timer >= this.m_TimerLimit - 20f)
		{
			this.m_IsApproaching = true;
			if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Easy)
			{
				this.OnApproaching.Send(this);
			}
		}
		if (GameManager.Instance.Player.CurrentState != State.Player.Cutscene && GameManager.Instance.Player.CurrentState != State.Player.CutscenePeek && GameManager.Instance.Player.CurrentState != State.Player.Ability && GameManager.Instance.Player.BattleStatus == BattleStatus.None && !GameManager.Instance.Player.IsUnderWanter && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
		{
			this.m_Timer += Time.deltaTime;
		}
	}

	public void CheckAvailability()
	{
		bool flag;
		flag = true;
		Section[] allSections;
		allSections = GameManager.Instance.SectionManager.GetAllSections();
		if (allSections != null && allSections.Length != 0)
		{
			foreach (Section section in allSections)
			{
				if (section.IsActive && section.SectionControllers != null)
				{
					SectionController[] sectionControllers;
					sectionControllers = section.SectionControllers;
					for (int j = 0; j < sectionControllers.Length; j = unchecked(j + 1))
					{
						if (sectionControllers[j] is SectionInkDemonController sectionInkDemonController)
						{
							if (!sectionInkDemonController.IsAvailable)
							{
								flag = false;
							}
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			this.SetActive(flag);
		}
		else
		{
			this.SetActive(active: false);
		}
	}

	public void ResetTimer()
	{
		this.m_Timer = 0f;
		this.m_TimerLimit = TimerCheck.InkDemon();
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	public void Clear()
	{
		this.IsActive = false;
		this.m_IsApproaching = false;
		this.m_Timer = 0f;
	}

	protected override void OnDisposed()
	{
		this.OnApproaching = null;
		base.OnDisposed();
	}
}
