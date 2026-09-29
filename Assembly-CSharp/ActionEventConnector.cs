using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(110)]
public class ActionEventConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEventCommand[] m_Senders;

	[SerializeField]
	private ActionEventExecutionType m_ExecutionType;

	[SerializeField]
	private bool m_FailureOnReset;

	[SerializeField]
	private bool m_DisableOnSuccess;

	[SerializeField]
	private bool m_OnStart = true;

	[SerializeField]
	private bool m_ForceInactive;

	[SerializeField]
	private ActionEventCommand[] m_Recievers;

	[SerializeField]
	private bool m_HasExecutionDelay;

	[SerializeField]
	private float m_ExecutionDelay;

	[SerializeField]
	private bool m_IsExecutionDelaySingle;

	private List<bool> m_ExecutionSequence = new List<bool>();

	private int m_ExecutionSequenceOrder = -1;

	private Sequencer m_Sequencer;

	public ActionEventCommand[] Senders => this.m_Senders;

	public ActionEventCommand[] Recievers => this.m_Recievers;

	public ActionEventExecutionType ExecutionType => this.m_ExecutionType;

	public bool HasExecutionDelay => this.m_HasExecutionDelay;

	public event EventHandler OnEvent;

	public event EventHandler OnComplete;

	public override void Start()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			this.m_ExecutionSequence.Add(item: false);
		}
		this.AddListeners();
		this.m_Sequencer = new Sequencer();
		if (!this.m_OnStart)
		{
			this.Disable();
		}
		if (this.m_ForceInactive)
		{
			this.ForceInactive();
		}
	}

	private void HandleActionEvent(object sender, EventArgs e)
	{
		ActionEventCommand actionEventCommand;
		actionEventCommand = (ActionEventCommand)sender;
		actionEventCommand.isReady = true;
		bool flag;
		flag = false;
		if (this.m_ExecutionType == ActionEventExecutionType.Any)
		{
			flag = true;
		}
		else if (this.m_ExecutionType == ActionEventExecutionType.All)
		{
			flag = this.CheckAllStatus(actionEventCommand.ID);
		}
		else if (this.m_ExecutionType == ActionEventExecutionType.Sequence)
		{
			flag = this.CheckSequenceStatus(actionEventCommand.ID);
		}
		if (flag)
		{
			this.ExecuteRecievers();
		}
		this.OnEvent.Send(actionEventCommand);
	}

	public void ForceInactive()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			this.m_Senders[i].ActionEventController.Content?.ForceInactive();
		}
	}

	public void ForceActive()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			this.m_Senders[i].ActionEventController.Content?.ForceActive();
		}
	}

	public void ForceActivateComplete()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			this.m_Senders[i].ActionEventController.Content?.ForceActivateComplete();
		}
	}

	public void Enable()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			this.m_Senders[i]?.ActionEventController.Content?.Enable();
		}
	}

	public void Disable()
	{
		if (this.m_Senders != null)
		{
			for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
			{
				this.m_Senders[i]?.ActionEventController.Content?.Disable();
			}
		}
	}

	public void DisableRecievers()
	{
		if (this.m_Recievers != null)
		{
			for (int i = 0; i < this.m_Recievers.Length; i = unchecked(i + 1))
			{
				this.m_Recievers[i]?.ActionEventController.Content?.Disable();
			}
		}
	}

	private bool CheckAllStatus(int index)
	{
		bool result;
		result = true;
		for (int i = 0; i < this.m_ExecutionSequence.Count; i = unchecked(i + 1))
		{
			if (!this.m_ExecutionSequence[i])
			{
				if (index == i)
				{
					this.m_ExecutionSequence[i] = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	private bool CheckSequenceStatus(int index)
	{
		bool result;
		result = true;
		this.m_ExecutionSequence[index] = true;
		unchecked
		{
			this.m_ExecutionSequenceOrder++;
			if (this.m_ExecutionSequenceOrder < this.m_ExecutionSequence.Count)
			{
				if (this.m_ExecutionSequenceOrder != index)
				{
					this.ResetExecutionSequence();
					result = false;
				}
				else
				{
					for (int i = 0; i < this.m_ExecutionSequence.Count; i++)
					{
						bool flag;
						flag = this.m_ExecutionSequence[i];
						if (i <= this.m_ExecutionSequenceOrder)
						{
							if (!flag)
							{
								result = false;
								this.ResetExecutionSequence();
								break;
							}
							continue;
						}
						result = false;
						break;
					}
				}
			}
			return result;
		}
	}

	private void ResetExecutionSequence()
	{
		for (int i = 0; i < this.m_ExecutionSequence.Count; i = unchecked(i + 1))
		{
			this.m_ExecutionSequence[i] = false;
		}
		if (this.m_FailureOnReset)
		{
			this.m_ExecutionSequenceOrder = -1;
			ActionEventCommand[] senders;
			senders = this.m_Senders;
			foreach (ActionEventCommand obj in senders)
			{
				obj.isReady = false;
				obj.ActionEventController.Content.ForceDeactivate();
			}
		}
	}

	private void ExecuteRecievers()
	{
		unchecked
		{
			if (this.m_DisableOnSuccess)
			{
				this.RemoveListeners();
				for (int i = 0; i < this.m_Senders.Length; i++)
				{
					this.m_Senders[i].ActionEventController.Content.Disable();
				}
			}
			if (this.m_HasExecutionDelay)
			{
				this.m_Sequencer.New();
			}
			if (this.m_Recievers == null || this.m_Recievers.Length == 0)
			{
				this.SendOnComplete();
				return;
			}
			for (int j = 0; j < this.m_Recievers.Length; j++)
			{
				ActionEventCommand actionEventCommand;
				actionEventCommand = this.m_Recievers[j];
				if (this.m_HasExecutionDelay)
				{
					float atPosition;
					atPosition = this.m_ExecutionDelay;
					if (!this.m_IsExecutionDelaySingle)
					{
						atPosition = this.m_ExecutionDelay + this.m_ExecutionDelay * (float)j;
					}
					this.m_Sequencer.Insert(atPosition, actionEventCommand.Execute);
					if (j == this.m_Recievers.Length - 1)
					{
						this.m_Sequencer.Insert(atPosition, this.SendOnComplete);
					}
				}
				else
				{
					actionEventCommand.Execute();
					this.SendOnComplete();
				}
			}
		}
	}

	private void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	private void AddListeners()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			ActionEventCommand obj;
			obj = this.m_Senders[i];
			obj.OnEvent += this.HandleActionEvent;
			obj.AddListeners();
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < this.m_Senders.Length; i = unchecked(i + 1))
		{
			ActionEventCommand obj;
			obj = this.m_Senders[i];
			obj.OnEvent -= this.HandleActionEvent;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ActionEventConnector Icon", allowScaling: true);
	}

	private void OnDrawGizmosSelected()
	{
		unchecked
		{
			if (this.m_Senders != null)
			{
				Gizmos.color = Color.white;
				for (int i = 0; i < this.m_Senders.Length; i++)
				{
					if (this.m_Senders[i].ActionEventController != null)
					{
						Gizmos.DrawLine(base.transform.position, this.m_Senders[i].ActionEventController.InteractableTransform.position);
					}
				}
			}
			if (this.m_Recievers == null)
			{
				return;
			}
			Gizmos.color = Color.cyan;
			for (int j = 0; j < this.m_Recievers.Length; j++)
			{
				if (this.m_Recievers[j].ActionEventController != null)
				{
					Gizmos.DrawLine(base.transform.position, this.m_Recievers[j].ActionEventController.InteractableTransform.position);
				}
			}
		}
	}
}
