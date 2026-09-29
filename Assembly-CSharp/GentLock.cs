using System;
using DG.Tweening;
using UnityEngine;

public class GentLock : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("GentLock Identifier")]
	[SerializeField]
	protected GentLockID m_GentLockID;

	[Header("GentLock Content")]
	[SerializeField]
	private GentLockContent m_Content;

	[Header("GentLock Complete")]
	[SerializeField]
	private GameObject m_CompleteContent;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Status")]
	[SerializeField]
	private bool m_OpenOnInitialize;

	public SectionID SectionID => this.m_SectionID;

	public GentLockID GentLockID => this.m_GentLockID;

	public GentLockContent Content { get; private set; }

	public bool IsActive { get; private set; }

	public bool IsActivated { get; private set; }

	public bool IsComplete { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentLockDirectory.ContainsKey(this.m_GentLockID))
		{
			if (!this.m_OpenOnInitialize)
			{
				this.AddContent();
			}
			else
			{
				this.AddCompleteContent();
			}
		}
		else
		{
			this.AddCompleteContent();
		}
	}

	private void AddContent()
	{
		this.Content = GameManager.Instance.AssetManager.CreateAsset<GentLockContent>(this.m_Content);
		this.Content.transform.SetParent(base.transform);
		this.Content.transform.localPosition = Vector3.zero;
		this.Content.transform.localEulerAngles = Vector3.zero;
		this.Content.OnForceComplete -= this.HandleContentOnForceComplete;
		this.Content.OnForceComplete += this.HandleContentOnForceComplete;
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		this.Content.Activator.OnActivated += this.HandleActivatorOnActivated;
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void HandleContentOnForceComplete(object sender, EventArgs e)
	{
		this.Content.OnForceComplete -= this.HandleContentOnForceComplete;
		this.ForceComplete();
	}

	private void AddCompleteContent()
	{
		if (this.m_CompleteContent != null)
		{
			CutsceneComplete cutsceneComplete;
			cutsceneComplete = GameManager.Instance.AssetManager.CreateAsset<CutsceneComplete>(this.m_CompleteContent.GetComponent<CutsceneComplete>());
			cutsceneComplete.transform.SetParent(base.transform);
			cutsceneComplete.transform.localPosition = Vector3.zero;
			cutsceneComplete.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void Play()
	{
		this.Content.Activator.Play();
	}

	private void InternalPreparePlay()
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.GentLock);
		}
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.HideFirstPersonArms();
		GameManager.Instance.Player.CancelMovement();
		GameManager.Instance.Player.ResetAnimation();
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		Sequence sequence;
		sequence = DOTween.Sequence();
		sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.Content.StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.Content.StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.OnComplete(this.InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		this.Content.Director.Play();
		this.OnActivated.Send(this);
	}

	public void ForcePlay(double timeline)
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.GentLock);
		}
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		this.Content.Director.Play();
		this.Content.Director.PlayableDirector.time = timeline;
		this.OnActivated.Send(this);
	}

	private void DirectorOnComplete()
	{
		this.RemoveListeners();
		GameManager.Instance.ShowCrosshair();
		GentPipePowerIndicator[] componentsInChildren;
		componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].SetPowerLevel(0);
		}
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetPower(0);
		this.IsComplete = true;
		this.OnComplete.Send(this);
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.GentLock)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
	}

	public void ForceComplete()
	{
		this.DirectorOnComplete();
	}

	public void SetActive(bool active)
	{
		this.Content.Activator.SetActive(active);
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		GameManager.Instance.GameData.CurrentSave.DataDirectories.GentLockDirectory.Add(this.m_GentLockID, DataObject<GentLockID, GentLockDataObject>.Create(this.m_GentLockID));
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		this.InternalPreparePlay();
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		this.DirectorOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("GentLock :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	private void Update()
	{
		if (this.IsComplete || !this.IsActive || base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.Player.CurrentWeapon == null)
		{
			return;
		}
		if (!this.IsActivated)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power >= 4)
			{
				GameManager.Instance.HideInteractInvalid();
				this.IsActivated = true;
				this.SetActive(active: true);
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			this.IsActivated = false;
			this.Content.Activator.Action.ForceDisable();
			this.SetActive(active: false);
		}
		else if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power < 4)
		{
			this.IsActivated = false;
			this.Content.Activator.Action.ForceDisable();
			this.SetActive(active: false);
		}
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

	private void RemoveListeners()
	{
		if (this.Content != null)
		{
			this.Content.OnForceComplete -= this.HandleContentOnForceComplete;
			this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		}
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
