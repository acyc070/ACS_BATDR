using System;
using UnityEngine;

public class Ladder : ActionEventController<LadderContent, LadderData>
{
	[DisplayWithoutEdit]
	[Space]
	[SerializeField]
	private int m_ID;

	public int ID => this.m_ID;

	public int LadderIndex => (base.Content as LadderContent).LadderIndex;

	public int CurrentLadderIndex => (base.Content as LadderContent).CurrentLadderIndex;

	public int CurrentLadderSide => (base.Content as LadderContent).LadderSide;

	public int ClimbDirection => (base.Content as LadderContent).ClimbDirection;

	public event EventHandler OnLadderBottomEnter;

	public event EventHandler OnLadderBottomExit;

	public event EventHandler OnLadderTopEnter;

	public event EventHandler OnLadderTopExit;

	public event EventHandler OnDeath;

	public void SetID(int id)
	{
		this.m_ID = id;
	}

	protected override void OnInitialized()
	{
		if (base.Content != null)
		{
			(base.Content as LadderContent).LadderBottom.OnEnter += this.HandleLadderBottomOnEnter;
			(base.Content as LadderContent).LadderBottom.OnExit += this.HandleLadderBottomOnExit;
			(base.Content as LadderContent).LadderTop.OnEnter += this.HandleLadderTopOnEnter;
			(base.Content as LadderContent).LadderTop.OnExit += this.HandleLadderTopOnExit;
			(base.Content as LadderContent).OnDeath += this.HandleLadderOnDeath;
		}
	}

	public void ForceEnterBottom(LadderDataObject dataObject)
	{
		(base.Content as LadderContent).ForceEnterBottom(dataObject);
	}

	public void ForceEnterTop(LadderDataObject dataObject)
	{
		(base.Content as LadderContent).ForceEnterTop(dataObject);
	}

	private void HandleLadderBottomOnEnter(object sender, EventArgs e)
	{
		this.OnLadderBottomEnter.Send(this);
	}

	private void HandleLadderBottomOnExit(object sender, EventArgs e)
	{
		this.OnLadderBottomExit.Send(this);
	}

	private void HandleLadderTopOnEnter(object sender, EventArgs e)
	{
		this.OnLadderTopEnter.Send(this);
	}

	private void HandleLadderTopOnExit(object sender, EventArgs e)
	{
		this.OnLadderTopExit.Send(this);
	}

	private void HandleLadderOnDeath(object sender, EventArgs e)
	{
		this.OnDeath.Send(this);
	}

	protected override void OnDisposed()
	{
		if (base.Content != null)
		{
			(base.Content as LadderContent).LadderBottom.OnEnter -= this.HandleLadderBottomOnEnter;
			(base.Content as LadderContent).LadderBottom.OnExit -= this.HandleLadderBottomOnExit;
			(base.Content as LadderContent).LadderTop.OnEnter -= this.HandleLadderTopOnEnter;
			(base.Content as LadderContent).LadderTop.OnExit -= this.HandleLadderTopOnExit;
			(base.Content as LadderContent).OnDeath -= this.HandleLadderOnDeath;
		}
		this.OnLadderBottomEnter = null;
		this.OnLadderBottomExit = null;
		this.OnLadderTopEnter = null;
		this.OnLadderTopExit = null;
		this.OnDeath = null;
		base.OnDisposed();
	}
}
