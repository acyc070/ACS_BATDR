using System;
using UnityEngine;

public class ObjectivePictureFrame : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Picture Frame")]
	[SerializeField]
	private PictureFrame m_PictureFrame;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectivePictureFrame :: InternalInitialize", this, JDebug.JDebugType.Objectives);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
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
			this.m_PictureFrame.OnComplete -= this.HandlePictureFrameOnComplete;
			this.m_PictureFrame.OnComplete += this.HandlePictureFrameOnComplete;
			this.m_PictureFrame.Initialize();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectivePictureFrame :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	protected override void InternalForceComplete()
	{
		JDebug.Log("ObjectivePictureFrame :: InternalForceComplete", this, JDebug.JDebugType.Objectives);
		this.m_PictureFrame.ForceComplete();
		base.SendOnComplete();
	}

	private void HandlePictureFrameOnComplete(object sender, EventArgs e)
	{
		this.m_PictureFrame.OnComplete -= this.HandlePictureFrameOnComplete;
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		this.m_PictureFrame.OnComplete -= this.HandlePictureFrameOnComplete;
		base.RemoveListeners();
	}
}
