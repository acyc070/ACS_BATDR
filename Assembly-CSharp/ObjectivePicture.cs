using System;
using UnityEngine;

public class ObjectivePicture : Objective
{
	[SerializeField]
	private Picture m_Picture;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		this.m_Picture.OnActivate += this.HandleOnActivate;
	}

	private void HandleOnActivate(object sender, EventArgs e)
	{
		this.m_Picture.OnActivate -= this.HandleOnActivate;
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		this.m_Picture.OnActivate -= this.HandleOnActivate;
	}
}
