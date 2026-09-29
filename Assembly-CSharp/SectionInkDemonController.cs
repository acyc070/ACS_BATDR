using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionInkDemonController : SectionController
{
	[SerializeField]
	private Requirements m_Requirements;

	public bool IsAvailable { get; private set; }

	protected override IEnumerator InternalInitialize()
	{
		if (this.m_Requirements != null)
		{
			if (!this.CheckStatus())
			{
				GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
				GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
				if (GameManager.Instance.InkDemonManager == null)
				{
					GameManager.Instance.InkDemonManager = InkDemonManager.Create();
				}
				GameManager.Instance.InkDemonManager.CheckAvailability();
			}
		}
		else
		{
			this.IsAvailable = false;
			if (GameManager.Instance.InkDemonManager == null)
			{
				GameManager.Instance.InkDemonManager = InkDemonManager.Create();
			}
			GameManager.Instance.InkDemonManager.CheckAvailability();
		}
		yield return null;
	}

	private void AllowInkDemonSpawning()
	{
		this.IsAvailable = true;
		if (GameManager.Instance.InkDemonManager == null)
		{
			GameManager.Instance.InkDemonManager = InkDemonManager.Create();
		}
		if (GameManager.Instance.InkDemonManager.Timer >= GameManager.Instance.InkDemonManager.TimerLimit)
		{
			GameManager.Instance.InkDemonManager.ResetTimer();
		}
		GameManager.Instance.InkDemonManager.CheckAvailability();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SectionInkDemonController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
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
			this.AllowInkDemonSpawning();
		}
		return flag;
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
