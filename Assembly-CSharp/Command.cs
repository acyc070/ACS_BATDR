using System;
using UnityEngine;

[Serializable]
public class Command<T, O> : Command where T : IConvertible where O : ICommand
{
	[SerializeField]
	protected T m_CommandType;

	[SerializeField]
	protected O m_CommandObject;

	[SerializeField]
	protected bool m_AutoComplete;

	public sealed override object CommandType => this.m_CommandType;

	public sealed override object CommandObject => this.m_CommandObject;

	public sealed override bool AutoComplete => this.m_AutoComplete;

	public sealed override void Execute()
	{
		if (!base.isExecuted && !base.isComplete && this.m_CommandObject != null)
		{
			this.InternalExecute();
			base.SendOnExecuted();
		}
	}

	protected virtual void InternalExecute()
	{
	}

	public sealed override void ForceComplete()
	{
		this.InternalForceComplete();
		base.SendOnComplete();
	}

	protected virtual void InternalForceComplete()
	{
	}
}
public abstract class Command
{
	public bool isExecuted { get; private set; }

	public bool isComplete { get; private set; }

	public abstract object CommandType { get; }

	public abstract object CommandObject { get; }

	public abstract bool AutoComplete { get; }

	public event EventHandler OnExecuted;

	public event EventHandler OnComplete;

	public void SendOnExecuted()
	{
		this.isExecuted = true;
		this.OnExecuted.Send(this);
	}

	public void SendOnComplete()
	{
		this.isExecuted = true;
		this.isComplete = true;
		this.OnComplete.Send(this);
	}

	public abstract void Execute();

	public abstract void ForceComplete();
}
