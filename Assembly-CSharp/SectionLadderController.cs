using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionLadderController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private LadderGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public LadderGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int activeID;
		activeID = -1;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			LadderGroup group;
			group = this.m_Group[i];
			LadderDataObject dataObject;
			dataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(this.m_SectionID, group.ID);
			if (dataObject == null)
			{
				dataObject = DataObject<int, LadderDataObject>.Create(group.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, dataObject);
			}
			else if (dataObject.ClimbState == 1)
			{
				activeID = dataObject.ID;
			}
			group.Controller.OnLadderBottomEnter -= this.HandleLadderBottomOnEnter;
			group.Controller.OnLadderBottomEnter += this.HandleLadderBottomOnEnter;
			group.Controller.OnLadderBottomExit -= this.HandleLadderBottomOnExit;
			group.Controller.OnLadderBottomExit += this.HandleLadderBottomOnExit;
			group.Controller.OnLadderTopEnter -= this.HandleLadderTopOnEnter;
			group.Controller.OnLadderTopEnter += this.HandleLadderTopOnEnter;
			group.Controller.OnLadderTopExit -= this.HandleLadderTopOnExit;
			group.Controller.OnLadderTopExit += this.HandleLadderTopOnExit;
			group.Controller.OnDeath -= this.HandleLadderOnDeath;
			group.Controller.OnDeath += this.HandleLadderOnDeath;
			group.Controller.Initialize();
			yield return null;
			if (group.ID != activeID)
			{
				continue;
			}
			if (dataObject.ClimbDirection == 1)
			{
				if (dataObject.LadderIndex >= group.Controller.LadderIndex)
				{
					dataObject.SetClimbDirection(2);
					group.Controller.ForceEnterTop(dataObject);
				}
				else
				{
					group.Controller.ForceEnterBottom(dataObject);
				}
			}
			else if (dataObject.ClimbDirection == 2)
			{
				if (dataObject.LadderIndex <= 0)
				{
					dataObject.SetClimbDirection(1);
					group.Controller.ForceEnterBottom(dataObject);
				}
				else
				{
					group.Controller.ForceEnterTop(dataObject);
				}
			}
		}
		yield return null;
	}

	private void HandleLadderBottomOnEnter(object sender, EventArgs e)
	{
		Ladder ladder;
		ladder = sender as Ladder;
		LadderDataObject ladderDataObject;
		ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(this.m_SectionID, ladder.ID);
		if (ladderDataObject == null)
		{
			ladderDataObject = DataObject<int, LadderDataObject>.Create(ladder.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, ladderDataObject);
		}
		ladderDataObject.SetClimbState(isClimbing: true);
		ladderDataObject.SetClimbDirection(1);
		ladderDataObject.SetLadderSide(0);
	}

	private void HandleLadderBottomOnExit(object sender, EventArgs e)
	{
		this.ExitLadder(sender as Ladder);
	}

	private void HandleLadderTopOnEnter(object sender, EventArgs e)
	{
		Ladder ladder;
		ladder = sender as Ladder;
		LadderDataObject ladderDataObject;
		ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(this.m_SectionID, ladder.ID);
		if (ladderDataObject == null)
		{
			ladderDataObject = DataObject<int, LadderDataObject>.Create(ladder.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, ladderDataObject);
		}
		ladderDataObject.SetClimbState(isClimbing: true);
		ladderDataObject.SetClimbDirection(2);
		ladderDataObject.SetLadderSide(0);
	}

	private void HandleLadderTopOnExit(object sender, EventArgs e)
	{
		this.ExitLadder(sender as Ladder);
	}

	private void HandleLadderOnDeath(object sender, EventArgs e)
	{
		this.ExitLadder(sender as Ladder);
	}

	private void ExitLadder(Ladder ladder)
	{
		LadderDataObject ladderDataObject;
		ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(this.m_SectionID, ladder.ID);
		if (ladderDataObject != null)
		{
			ladderDataObject.SetClimbState(isClimbing: false);
			ladderDataObject.SetClimbDirection(0);
		}
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			LadderGroup obj;
			obj = this.m_Group[i];
			obj.Controller.OnLadderBottomEnter -= this.HandleLadderBottomOnEnter;
			obj.Controller.OnLadderBottomExit -= this.HandleLadderBottomOnExit;
			obj.Controller.OnLadderTopEnter -= this.HandleLadderTopOnEnter;
			obj.Controller.OnLadderTopExit -= this.HandleLadderTopOnExit;
			obj.Controller.OnDeath -= this.HandleLadderOnDeath;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			LadderGroup ladderGroup;
			ladderGroup = this.m_Group[i];
			LadderDataObject ladderDataObject;
			ladderDataObject = (LadderDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LadderDataObject>(this.m_SectionID, ladderGroup.ID);
			if (ladderDataObject != null && ladderDataObject.ClimbState == 1)
			{
				ladderDataObject.SetLadderIndex(ladderGroup.Controller.CurrentLadderIndex);
				ladderDataObject.SetLadderSide(ladderGroup.Controller.CurrentLadderSide);
				ladderDataObject.SetClimbDirection(ladderGroup.Controller.ClimbDirection);
				ladderDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				ladderDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
