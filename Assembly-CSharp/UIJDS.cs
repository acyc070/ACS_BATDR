using System;
using UnityEngine;

public class UIJDS : UIController
{
	[SerializeField]
	private VideoController m_Video;

	protected override void OnInitialized(object _data)
	{
		this.m_Video.SetCamera(GameManager.Instance.UIManager.Camera);
	}

	public override void PlayIn()
	{
		this.m_Video.Play();
		this.m_Video.OnComplete -= this.HandleVideoOnComplete;
		this.m_Video.OnComplete += this.HandleVideoOnComplete;
		this.PlayInComplete();
	}

	private void HandleVideoOnComplete(object sender, EventArgs e)
	{
		this.m_Video.OnComplete -= this.HandleVideoOnComplete;
		this.PlayOut();
	}

	private void Update()
	{
		if (!base.IsDisposed && (Input.GetMouseButtonDown(2) || PlayerInput.ControllerRightBumper()))
		{
			this.m_Video.OnComplete -= this.HandleVideoOnComplete;
			this.m_Video.Stop();
			this.PlayOut();
		}
	}

	protected override void OnDisposed()
	{
		this.m_Video.OnComplete -= this.HandleVideoOnComplete;
		base.OnDisposed();
	}
}
