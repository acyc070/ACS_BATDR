using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionPeekController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private PeekGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public PeekGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int num;
		num = -1;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			PeekGroup peekGroup;
			peekGroup = this.m_Group[i];
			PeekDataObject peekDataObject;
			peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(this.m_SectionID, peekGroup.ID);
			if (peekDataObject == null)
			{
				peekDataObject = DataObject<int, PeekDataObject>.Create(peekGroup.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, peekDataObject);
			}
			else if (peekDataObject.PeekState == 1)
			{
				num = peekDataObject.ID;
			}
			peekGroup.Controller.OnEnter -= this.HandlePeekOnEnter;
			peekGroup.Controller.OnEnter += this.HandlePeekOnEnter;
			peekGroup.Controller.OnExit -= this.HandlePeekOnExit;
			peekGroup.Controller.OnExit += this.HandlePeekOnExit;
			peekGroup.Controller.Initialize();
			if (peekGroup.ID == num)
			{
				peekGroup.Controller.ForceEnter(peekDataObject);
			}
		}
		yield return null;
	}

	private void HandlePeekOnEnter(object sender, EventArgs e)
	{
		Peek peek;
		peek = sender as Peek;
		PeekDataObject peekDataObject;
		peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(this.m_SectionID, peek.ID);
		if (peekDataObject == null)
		{
			peekDataObject = DataObject<int, PeekDataObject>.Create(peek.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, peekDataObject);
		}
		peekDataObject.SetIndex(peek.CurrentIndex);
		peekDataObject.SetPeekState(isPeeking: true);
	}

	private void HandlePeekOnExit(object sender, EventArgs e)
	{
		((PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(this.m_SectionID, (sender as Peek).ID))?.SetPeekState(isPeeking: false);
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			PeekGroup obj;
			obj = this.m_Group[i];
			obj.Controller.OnEnter -= this.HandlePeekOnEnter;
			obj.Controller.OnExit -= this.HandlePeekOnExit;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			PeekGroup peekGroup;
			peekGroup = this.m_Group[i];
			PeekDataObject peekDataObject;
			peekDataObject = (PeekDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PeekDataObject>(this.m_SectionID, peekGroup.ID);
			if (peekDataObject != null && peekDataObject.PeekState == 1)
			{
				peekDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				peekDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
