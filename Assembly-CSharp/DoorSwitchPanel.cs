using System;
using DG.Tweening;
using UnityEngine;

public class DoorSwitchPanel : JMonoBehaviour
{
	[Serializable]
	private class DoorSwitchGroup
	{
		public DoorID DoorID;

		public LightFixture LightFixture;
	}

	[Header("Button")]
	[SerializeField]
	private Interactable m_InteractableButton;

	[SerializeField]
	private Transform m_Button;

	[Header("Door Switch Group")]
	[SerializeField]
	private DoorSwitchGroup[] m_Group;

	private Sequence m_ButtonSequence;

	public void Initialize()
	{
		Door[] componentsInChildren;
		componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			DoorSwitchGroup doorSwitchGroup;
			doorSwitchGroup = this.m_Group[i];
			Door[] array;
			array = componentsInChildren;
			foreach (Door door in array)
			{
				if (door.ID == doorSwitchGroup.DoorID)
				{
					if (door.Data.Status == DoorStatus.Open)
					{
						doorSwitchGroup.LightFixture.SetEmission(1f);
					}
					else if (door.Data.Status == DoorStatus.Closed)
					{
						doorSwitchGroup.LightFixture.SetEmission(0f);
					}
				}
			}
		}
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		this.m_InteractableButton.OnInteract += this.HandleButtonOnInteract;
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		Door[] componentsInChildren;
		componentsInChildren = base.transform.root.GetComponentsInChildren<Door>(includeInactive: true);
		int num;
		num = 0;
		bool flag;
		flag = false;
		unchecked
		{
			Door[] array;
			for (int i = 0; i < this.m_Group.Length; i++)
			{
				DoorSwitchGroup doorSwitchGroup;
				doorSwitchGroup = this.m_Group[i];
				array = componentsInChildren;
				foreach (Door door in array)
				{
					if (door.ID == doorSwitchGroup.DoorID && door.Data.Status == DoorStatus.Open)
					{
						door.Content.ForceDeactivate();
						doorSwitchGroup.LightFixture.SetEmission(0f);
						flag = true;
						break;
					}
				}
				num++;
				if (flag)
				{
					break;
				}
			}
			if (num >= this.m_Group.Length)
			{
				num = 0;
			}
			this.m_Group[num].LightFixture.SetEmission(1f);
			array = componentsInChildren;
			foreach (Door door2 in array)
			{
				if (door2.ID == this.m_Group[num].DoorID)
				{
					door2.Content.ForceActivate();
					break;
				}
			}
			this.ResetSequence();
			this.m_ButtonSequence.Insert(0f, this.m_Button.DOLocalMoveX(0.23f, 0.2f).SetEase(Ease.Linear));
			this.m_ButtonSequence.Insert(0.2f, this.m_Button.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutSine));
			this.m_ButtonSequence.OnComplete(() =>
			{
				this.m_InteractableButton.OnInteract += this.HandleButtonOnInteract;
				this.m_InteractableButton.ResetAction();
			});
		}
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_ButtonSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_ButtonSequence != null)
		{
			this.m_ButtonSequence.Kill();
			this.m_ButtonSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		base.OnDisposed();
	}
}
