using System;
using UnityEngine;

public class ObjectiveCutscene : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_Cutscene;

	[Header("Activation Options")]
	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_CanInterrupt;

	protected override void InternalUpdate()
	{
		if (this.m_CanInterrupt || GameManager.Instance.Player == null)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat || GameManager.Instance.Player.BattleStatus != BattleStatus.None)
		{
			if (this.m_Cutscene.Content != null && !this.m_Cutscene.Content.Director.IsPlaying && this.m_Cutscene.Content.Activator.IsActive)
			{
				if (this.m_Cutscene.Content.Activator.Action != null)
				{
					this.m_Cutscene.Content.Activator.Action.ForceDisable();
				}
				this.m_Cutscene.SetActive(active: false);
			}
		}
		else if (this.m_Cutscene.Content != null && !this.m_Cutscene.Content.Director.IsPlaying && this.CheckStatusSimple() && !this.m_Cutscene.Content.Activator.IsActive)
		{
			this.m_Cutscene.SetActive(active: true);
		}
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("Cutscene :: InternalInitialize", this, JDebug.JDebugType.Cutscene);
		if (this.m_Cutscene == null)
		{
			return;
		}
		this.RemoveListeners();
		CutsceneDataObject cutsceneDataObject;
		cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(this.m_Cutscene.SectionID, this.m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status == CutsceneStatus.Complete)
			{
				base.SendOnComplete();
			}
			else if (cutsceneDataObject.Status == CutsceneStatus.Active)
			{
				if (this.m_ClearNavigation)
				{
					GameManager.Instance.ClearNavigation();
				}
				this.m_Cutscene.OnComplete += this.HandleCutsceneOnComplete;
				this.m_Cutscene.SetActive(active: false);
				if (this.m_Cutscene.Content != null)
				{
					this.m_Cutscene.Content.Initialize();
					if (this.m_Cutscene.Content.Activator != null && this.m_Cutscene.Content.Activator.Action != null)
					{
						this.m_Cutscene.Content.Activator.Action.Dispose();
					}
				}
				this.m_Cutscene.ForcePlay(cutsceneDataObject.Timeline);
			}
			else
			{
				this.CheckInitializer();
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(this.m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_Cutscene.SectionID, cutsceneDataObject);
			this.CheckInitializer();
		}
	}

	private void CheckInitializer()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private bool CheckStatusSimple()
	{
		bool result;
		result = true;
		if (this.m_Requirements != null)
		{
			result = this.m_Requirements.IsComplete();
		}
		return result;
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
			this.InitializeCutscene();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}

	private void InitializeCutscene()
	{
		this.m_Cutscene.OnActivated += this.HandleCutsceneOnActivated;
		this.m_Cutscene.OnComplete += this.HandleCutsceneOnComplete;
		this.m_Cutscene.SetActive(active: true);
		InteractableInactive componentInChildren;
		componentInChildren = this.m_Cutscene.GetComponentInChildren<InteractableInactive>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.InitializeInactive();
		}
		if (this.m_Cutscene.Content != null)
		{
			this.m_Cutscene.Content.Initialize();
			if (this.m_Cutscene.Content.Activator.ActivateType == CutsceneActivateType.Callback)
			{
				this.m_Cutscene.Play();
			}
		}
	}

	protected override void InternalEnable()
	{
		JDebug.Log("Cutscene :: InternalEnable", this, JDebug.JDebugType.Cutscene);
		if (this.m_Cutscene == null)
		{
			return;
		}
		this.RemoveListeners();
		CutsceneDataObject cutsceneDataObject;
		cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(this.m_Cutscene.SectionID, this.m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status == CutsceneStatus.Complete)
			{
				base.SendOnComplete();
			}
			else
			{
				this.m_Cutscene.OnActivated += this.HandleCutsceneEnableOnActivated;
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(this.m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_Cutscene.SectionID, cutsceneDataObject);
			this.m_Cutscene.OnActivated += this.HandleCutsceneEnableOnActivated;
		}
	}

	private void HandleCutsceneEnableOnActivated(object sender, EventArgs e)
	{
		this.m_Cutscene.OnActivated -= this.HandleCutsceneEnableOnActivated;
		base.SendOnComplete();
	}

	private void HandleCutsceneOnActivated(object sender, EventArgs e)
	{
		this.m_Cutscene.OnActivated -= this.HandleCutsceneOnActivated;
		CutsceneDataObject cutsceneDataObject;
		cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(this.m_Cutscene.SectionID, this.m_Cutscene.CutsceneID);
		if (cutsceneDataObject == null)
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(this.m_Cutscene.CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.Active);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_Cutscene.SectionID, cutsceneDataObject);
		}
		else
		{
			cutsceneDataObject.SetStatus(CutsceneStatus.Active);
		}
		if (this.m_ClearNavigation)
		{
			GameManager.Instance.ClearNavigation();
		}
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		this.m_Cutscene.OnComplete -= this.HandleCutsceneOnComplete;
		this.CompleteData();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("Cutscene :: InternalForceComplete", this, JDebug.JDebugType.Cutscene);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.RemoveListeners();
		if (this.m_Cutscene.ResetOnComplete)
		{
			this.m_Cutscene.OnActivated += this.HandleCutsceneOnActivated;
			this.m_Cutscene.OnComplete += this.HandleCutsceneOnComplete;
			this.m_Cutscene.SetActive(active: true);
		}
		else
		{
			this.m_Cutscene.SetActive(active: false);
			this.m_Cutscene.ForceComplete();
			base.SendOnComplete();
		}
	}

	protected override void InternalComplete()
	{
		JDebug.Log("Cutscene :: InternalComplete", this, JDebug.JDebugType.Cutscene);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.RemoveListeners();
	}

	private void CompleteData()
	{
		CutsceneDataObject cutsceneDataObject;
		cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(this.m_Cutscene.SectionID, this.m_Cutscene.CutsceneID);
		if (cutsceneDataObject != null)
		{
			cutsceneDataObject.SetStatus(CutsceneStatus.Complete);
			return;
		}
		cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(this.m_Cutscene.CutsceneID);
		cutsceneDataObject.SetStatus(CutsceneStatus.Complete);
		GameManager.Instance.GameData.CurrentSave.AddData(this.m_Cutscene.SectionID, cutsceneDataObject);
	}

	protected override void RemoveListeners()
	{
		if (this.m_Cutscene != null)
		{
			this.m_Cutscene.OnActivated -= this.HandleCutsceneEnableOnActivated;
			this.m_Cutscene.OnActivated -= this.HandleCutsceneOnActivated;
			this.m_Cutscene.OnComplete -= this.HandleCutsceneOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		base.OnDisposed();
	}
}
