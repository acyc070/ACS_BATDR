using System;
using UnityEngine;

public class ObjectiveController : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Data")]
	[SerializeField]
	private ObjectiveData m_Data;

	[Header("Commands")]
	[SerializeField]
	private ObjectiveCommand[] m_OnStartCommands;

	[SerializeField]
	private ObjectiveCommand[] m_OnExecuteCommands;

	[SerializeField]
	private ObjectiveCommand[] m_OnCompleteCommands;

	private int m_OnExecuteIndex;

	public int ID => this.m_ID;

	public ObjectiveData Data => this.m_Data;

	public bool IsActive { get; protected set; }

	public bool IsComplete { get; protected set; }

	public event EventHandler OnComplete;

	public void OnStart()
	{
		for (int i = 0; i < this.m_OnStartCommands.Length; i = unchecked(i + 1))
		{
			this.m_OnStartCommands[i].Execute();
		}
	}

	public void Execute()
	{
		this.IsActive = true;
		if (this.m_OnExecuteCommands.Length != 0)
		{
			for (int i = 0; i < this.m_OnExecuteCommands.Length; i = unchecked(i + 1))
			{
				ObjectiveCommand objectiveCommand;
				objectiveCommand = this.m_OnExecuteCommands[i];
				objectiveCommand.OnComplete += this.HandleCommandOnComplete;
				objectiveCommand.Execute();
				if (objectiveCommand.AutoComplete)
				{
					objectiveCommand.ForceComplete();
				}
			}
		}
		else
		{
			this.Complete();
		}
	}

	public void Complete()
	{
		for (int i = 0; i < this.m_OnCompleteCommands.Length; i = unchecked(i + 1))
		{
			this.m_OnCompleteCommands[i].Execute();
		}
		this.SetComplete();
		GameManager.Instance.CompleteObjective(this.m_Data);
	}

	public void ForceComplete()
	{
		unchecked
		{
			for (int i = 0; i < this.m_OnExecuteCommands.Length; i++)
			{
				ObjectiveCommand obj;
				obj = this.m_OnExecuteCommands[i];
				obj.OnComplete -= this.HandleCommandOnComplete;
				obj.ForceComplete();
			}
			for (int j = 0; j < this.m_OnCompleteCommands.Length; j++)
			{
				this.m_OnCompleteCommands[j].ForceComplete();
			}
			this.SetComplete();
		}
	}

	private void SetComplete()
	{
		this.IsComplete = true;
		this.OnComplete.Send(this);
	}

	private void HandleCommandOnComplete(object sender, EventArgs e)
	{
		((ObjectiveCommand)sender).OnComplete -= this.HandleCommandOnComplete;
		unchecked
		{
			this.m_OnExecuteIndex++;
			if (this.m_OnExecuteIndex >= this.m_OnExecuteCommands.Length)
			{
				this.Complete();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		base.OnDisposed();
	}
}
