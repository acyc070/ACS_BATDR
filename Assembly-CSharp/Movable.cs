using System;
using UnityEngine;

public class Movable : ActionEventController<MovableContent, MovableData>
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Movable Identifier")]
	[SerializeField]
	private MovableID m_ID;

	[Header("Movable Path")]
	[SerializeField]
	private MovablePath m_Path;

	public SectionID SectionID => this.m_SectionID;

	public MovableID ID => this.m_ID;

	public MovablePath Path => this.m_Path;

	public event EventHandler OnExit;

	public event EventHandler OnFrontEnter;

	public event EventHandler OnBackEnter;

	protected override void OnInitialized()
	{
		if (base.Content != null)
		{
			(base.Content as MovableContent).OnFrontEnter += this.HandleContentOnFrontEnter;
			(base.Content as MovableContent).OnBackEnter += this.HandleContentOnBackEnter;
			(base.Content as MovableContent).OnExit += this.HandleContentOnExit;
		}
	}

	public void SetStartLocation()
	{
		MovableContent movableContent;
		movableContent = base.Content as MovableContent;
		movableContent.SetStartLocation(this.m_Path.Path, this.m_Path.Path[base.Data.CurrentIndex]);
		if (base.Data.IsActive)
		{
			movableContent.Enable();
		}
		else
		{
			movableContent.Disable();
		}
	}

	public void ForceEnterFront(MovableDataObject dataObject)
	{
		(base.Content as MovableContent).ForceEnterFront(dataObject);
	}

	public void ForceEnterBack(MovableDataObject dataObject)
	{
		(base.Content as MovableContent).ForceEnterBack(dataObject);
	}

	private void HandleContentOnFrontEnter(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: true);
		base.Data.SetSide(0);
		this.OnFrontEnter.Send(this);
	}

	private void HandleContentOnBackEnter(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: true);
		base.Data.SetSide(1);
		this.OnBackEnter.Send(this);
	}

	private void HandleContentOnExit(object sender, EventArgs e)
	{
		base.Data.SetMoveState(canMove: false);
		base.Data.SetSide(0);
		this.OnExit.Send(this);
	}

	protected override void OnDisposed()
	{
		if (base.Content != null)
		{
			(base.Content as MovableContent).OnFrontEnter -= this.HandleContentOnFrontEnter;
			(base.Content as MovableContent).OnBackEnter -= this.HandleContentOnBackEnter;
			(base.Content as MovableContent).OnExit -= this.HandleContentOnExit;
		}
		this.OnExit = null;
		this.OnFrontEnter = null;
		this.OnBackEnter = null;
		base.OnDisposed();
	}
}
