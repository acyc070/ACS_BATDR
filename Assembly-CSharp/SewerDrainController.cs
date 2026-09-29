using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SewerDrainController : SectionController
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Button")]
	[SerializeField]
	private Interactable m_InteractableButton;

	[SerializeField]
	private Transform m_Button;

	[SerializeField]
	private LightFixture m_LightFixture;

	[Header("Plug")]
	[SerializeField]
	private Door m_Plug;

	[SerializeField]
	private GameObject m_PlugSparkles;

	[SerializeField]
	private Ladder m_PlugLadder;

	[SerializeField]
	private LightFlicker m_LightFlicker;

	[Header("Ink Door")]
	[SerializeField]
	private Door m_InkDoor;

	[Header("Ink Raft")]
	[SerializeField]
	private Transform m_InkRaft;

	[SerializeField]
	private Transform m_InkRaftTopLocation;

	[SerializeField]
	private Transform m_InkRaftBottomLocation;

	[SerializeField]
	private Ladder m_InkRaftLadder;

	[SerializeField]
	private Lever m_InkRaftLever;

	private Sequence m_ButtonSequence;

	public event EventHandler OnPush;

	protected override IEnumerator InternalInitialize()
	{
		if (this.m_InkDoor.Data.Status == DoorStatus.Open)
		{
			this.m_LightFixture.SetEmission(1f);
			this.m_PlugLadder.Content.Disable();
			if (this.m_Plug.Data.Status == DoorStatus.Open)
			{
				this.m_InkRaft.position = this.m_InkRaftTopLocation.position;
				this.m_InkRaftLadder.Content.Disable();
				this.m_InkRaftLever.Content.Enable();
				InteractableInactive componentInChildren;
				componentInChildren = this.m_InkRaftLever.GetComponentInChildren<InteractableInactive>(includeInactive: true);
				if (componentInChildren != null)
				{
					componentInChildren.InitializeInactive();
				}
			}
		}
		else
		{
			this.m_LightFixture.SetEmission(0f);
		}
		if (this.m_Plug.Data.Status != DoorStatus.Open)
		{
			this.m_Plug.OnActivate -= this.HandlePlugOnActivate;
			this.m_Plug.OnActivate += this.HandlePlugOnActivate;
		}
		else
		{
			this.m_Plug.Content.Disable();
		}
		this.m_Plug.Content.Disable();
		this.m_PlugSparkles.SetActive(value: false);
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
			this.m_InteractableButton.OnInteract += this.HandleButtonOnInteract;
		}
		yield return null;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SewerDrainController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
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
			this.m_InteractableButton.SetActive(active: false);
		}
		return flag;
	}

	private void HandlePlugOnActivate(object sender, EventArgs e)
	{
		this.m_Plug.OnActivate -= this.HandlePlugOnActivate;
		this.m_PlugSparkles.SetActive(value: false);
		this.m_LightFlicker.TurnOff(light: true);
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		this.m_InkDoor.OnActivated -= this.HandleDoorComplete;
		this.m_InkDoor.OnDeactivated -= this.HandleDoorComplete;
		if (this.m_InkDoor.Data.Status == DoorStatus.Open)
		{
			if (this.m_Plug.Data.Status == DoorStatus.Open)
			{
				this.m_InkRaft.position = this.m_InkRaftBottomLocation.position;
				this.m_InkRaftLadder.Content.Enable();
				this.m_InkRaftLever.Content.Disable();
			}
			this.m_PlugSparkles.SetActive(value: false);
			if (this.m_Plug.Data.Status == DoorStatus.Closed)
			{
				this.m_Plug.Content.Disable();
			}
			this.m_PlugLadder.Content.Disable();
			this.m_InkDoor.Content.ForceDeactivate();
			this.m_LightFixture.SetEmission(0f);
			this.m_InkDoor.OnDeactivated += this.HandleDoorComplete;
		}
		else
		{
			if (this.m_Plug.Data.Status == DoorStatus.Open)
			{
				this.m_InkRaft.position = this.m_InkRaftTopLocation.position;
				this.m_InkRaftLadder.Content.Disable();
				this.m_InkRaftLever.Content.Enable();
			}
			this.m_PlugSparkles.SetActive(value: false);
			if (this.m_Plug.Data.Status == DoorStatus.Closed)
			{
				this.m_Plug.Content.Disable();
			}
			this.m_PlugLadder.Content.Disable();
			this.m_InkDoor.Content.ForceActivate();
			this.m_LightFixture.SetEmission(1f);
			this.m_InkDoor.OnActivated += this.HandleDoorComplete;
		}
		this.ResetSequence();
		this.m_ButtonSequence.Insert(0f, this.m_Button.DOLocalMoveX(0.23f, 0.2f).SetEase(Ease.Linear));
		this.m_ButtonSequence.Insert(0.2f, this.m_Button.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutSine));
		this.OnPush.Send(this);
	}

	private void HandleDoorComplete(object sender, EventArgs e)
	{
		this.m_InkDoor.OnActivated -= this.HandleDoorComplete;
		this.m_InkDoor.OnDeactivated -= this.HandleDoorComplete;
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		this.m_InteractableButton.OnInteract += this.HandleButtonOnInteract;
		this.m_InteractableButton.ResetAction();
		if (this.m_InkDoor.Data.Status == DoorStatus.Closed)
		{
			if (this.m_Plug.Data.Status == DoorStatus.Closed)
			{
				this.m_Plug.Content.Enable();
				this.m_PlugSparkles.SetActive(value: true);
			}
			this.m_PlugLadder.Content.Enable();
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
		this.OnPush = null;
		this.KillSequence();
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.m_Plug.OnActivate -= this.HandlePlugOnActivate;
		this.m_InkDoor.OnActivated -= this.HandleDoorComplete;
		this.m_InkDoor.OnDeactivated -= this.HandleDoorComplete;
		this.m_InteractableButton.OnInteract -= this.HandleButtonOnInteract;
		base.OnDisposed();
	}
}
