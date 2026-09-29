using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionObjectiveController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private ObjectiveController[] m_PrimaryObjectives;

	[SerializeField]
	private ObjectiveController[] m_ActiveObjectives;

	private ObjectiveController m_CurrentPrimaryObjective;

	private int m_PrimaryObjectiveIndex;

	private int m_ActiveObjectiveIndex;

	public SectionID SectionID => this.m_SectionID;

	public bool IsPrimaryComplete { get; private set; }

	public bool IsActiveComplete { get; private set; }

	protected override IEnumerator InternalInitialize()
	{
		this.RemoveListeners();
		this.InitializeObjectives();
		this.InitializeActiveObjectives();
		yield return null;
	}

	private void InitializeObjectives()
	{
		unchecked
		{
			if (this.m_PrimaryObjectives != null && this.m_PrimaryObjectives.Length != 0)
			{
				for (int i = 0; i < this.m_PrimaryObjectives.Length; i++)
				{
					ObjectiveController objectiveController;
					objectiveController = this.m_PrimaryObjectives[i];
					objectiveController.OnComplete -= this.HandlePrimaryObjectiveOnComplete;
					ObjectiveDataObject objectiveDataObject;
					objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(this.m_SectionID, objectiveController.ID);
					if (objectiveDataObject != null)
					{
						if (objectiveDataObject.Status == ObjectiveStatus.Complete)
						{
							objectiveController.ForceComplete();
							this.m_PrimaryObjectiveIndex++;
						}
						else
						{
							objectiveController.OnComplete += this.HandlePrimaryObjectiveOnComplete;
							objectiveController.OnStart();
						}
					}
					else
					{
						objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(objectiveController.ID);
						objectiveDataObject.SetStatus(ObjectiveStatus.Inactive);
						GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, objectiveDataObject);
						objectiveController.OnComplete += this.HandlePrimaryObjectiveOnComplete;
						objectiveController.OnStart();
					}
				}
				if (this.m_PrimaryObjectiveIndex < this.m_PrimaryObjectives.Length)
				{
					this.m_CurrentPrimaryObjective = this.m_PrimaryObjectives[this.m_PrimaryObjectiveIndex];
					this.ExecuteCurrentPrimaryObjective();
				}
				else
				{
					this.CheckPrimaryObjectivesComplete();
				}
			}
			else
			{
				this.AbortPrimaryObjectives();
			}
		}
	}

	private void InitializeActiveObjectives()
	{
		unchecked
		{
			if (this.m_ActiveObjectives != null && this.m_ActiveObjectives.Length != 0)
			{
				for (int i = 0; i < this.m_ActiveObjectives.Length; i++)
				{
					ObjectiveController objectiveController;
					objectiveController = this.m_ActiveObjectives[i];
					objectiveController.OnComplete -= this.HandleActiveObjectiveOnComplete;
					ObjectiveDataObject objectiveDataObject;
					objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(this.m_SectionID, objectiveController.ID);
					if (objectiveDataObject != null)
					{
						if (objectiveDataObject.Status == ObjectiveStatus.Complete)
						{
							objectiveController.ForceComplete();
							this.m_ActiveObjectiveIndex++;
						}
						else
						{
							objectiveController.OnComplete += this.HandleActiveObjectiveOnComplete;
							objectiveController.OnStart();
							objectiveController.Execute();
						}
					}
					else
					{
						objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(objectiveController.ID);
						objectiveDataObject.SetStatus(ObjectiveStatus.Active);
						GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, objectiveDataObject);
						objectiveController.OnComplete += this.HandleActiveObjectiveOnComplete;
						objectiveController.OnStart();
						objectiveController.Execute();
					}
				}
				if (this.m_ActiveObjectiveIndex >= this.m_ActiveObjectives.Length)
				{
					this.CheckActiveObjectivesComplete();
				}
			}
			else
			{
				this.AbortActiveObjectives();
			}
		}
	}

	private void HandlePrimaryObjectiveOnComplete(object sender, EventArgs e)
	{
		ObjectiveController objectiveController;
		objectiveController = (ObjectiveController)sender;
		objectiveController.OnComplete -= this.HandlePrimaryObjectiveOnComplete;
		this.UpdateObjectiveData(objectiveController.ID, ObjectiveStatus.Complete);
		this.CheckNextPrimaryObjective();
	}

	private void HandleActiveObjectiveOnComplete(object sender, EventArgs e)
	{
		ObjectiveController objectiveController;
		objectiveController = (ObjectiveController)sender;
		objectiveController.OnComplete -= this.HandleActiveObjectiveOnComplete;
		this.UpdateObjectiveData(objectiveController.ID, ObjectiveStatus.Complete);
		unchecked
		{
			this.m_ActiveObjectiveIndex++;
			if (this.m_ActiveObjectiveIndex >= this.m_ActiveObjectives.Length)
			{
				this.CheckActiveObjectivesComplete();
			}
		}
	}

	private void CheckActiveObjectivesComplete()
	{
		this.IsActiveComplete = true;
		this.CheckSectionControllerComplete();
	}

	private void UpdateObjectiveData(int id, ObjectiveStatus status)
	{
		ObjectiveDataObject objectiveDataObject;
		objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(this.m_SectionID, id);
		if (objectiveDataObject != null)
		{
			objectiveDataObject.SetStatus(status);
			return;
		}
		objectiveDataObject = DataObject<int, ObjectiveDataObject>.Create(id);
		objectiveDataObject.SetStatus(status);
		GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, objectiveDataObject);
	}

	private void CheckNextPrimaryObjective()
	{
		unchecked
		{
			this.m_PrimaryObjectiveIndex++;
			if (this.m_PrimaryObjectiveIndex < this.m_PrimaryObjectives.Length)
			{
				this.m_CurrentPrimaryObjective = this.m_PrimaryObjectives[this.m_PrimaryObjectiveIndex];
				if (this.m_CurrentPrimaryObjective.IsComplete)
				{
					this.m_CurrentPrimaryObjective.OnComplete -= this.HandlePrimaryObjectiveOnComplete;
					this.CheckNextPrimaryObjective();
				}
				else
				{
					this.ExecuteCurrentPrimaryObjective();
				}
			}
			else
			{
				this.CheckPrimaryObjectivesComplete();
			}
		}
	}

	private void CheckPrimaryObjectivesComplete()
	{
		this.IsPrimaryComplete = true;
		this.CheckSectionControllerComplete();
	}

	private void ExecuteCurrentPrimaryObjective()
	{
		this.m_CurrentPrimaryObjective.Execute();
		this.UpdateObjectiveData(this.m_CurrentPrimaryObjective.ID, ObjectiveStatus.Active);
	}

	private void CheckSectionControllerComplete()
	{
		if (this.IsPrimaryComplete && this.IsActiveComplete)
		{
			((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID))?.SetComplete();
			base.SendOnComplete();
			GameManager.Instance.CompleteSection();
		}
	}

	private void AbortPrimaryObjectives()
	{
		JDebug.Log("[Abort Primary Objectives] :: There are no m_PrimaryObjectives", this, JDebug.JDebugType.Objectives);
		this.IsPrimaryComplete = true;
		this.CheckSectionControllerComplete();
	}

	private void AbortActiveObjectives()
	{
		JDebug.Log("[Abort Active Objectives] :: There are no m_ActiveObjectives", this, JDebug.JDebugType.Objectives);
		this.IsActiveComplete = true;
		this.CheckSectionControllerComplete();
	}

	protected override void RemoveListeners()
	{
		unchecked
		{
			if (this.m_PrimaryObjectives != null && this.m_PrimaryObjectives.Length != 0)
			{
				for (int i = 0; i < this.m_PrimaryObjectives.Length; i++)
				{
					this.m_PrimaryObjectives[i].OnComplete -= this.HandlePrimaryObjectiveOnComplete;
				}
			}
			if (this.m_ActiveObjectives != null && this.m_ActiveObjectives.Length != 0)
			{
				for (int j = 0; j < this.m_ActiveObjectives.Length; j++)
				{
					this.m_ActiveObjectives[j].OnComplete -= this.HandleActiveObjectiveOnComplete;
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_CurrentPrimaryObjective = null;
	}
}
