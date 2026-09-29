using System;
using UnityEngine;

public class CutsceneComplete : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Requirement Complete")]
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public void Initialize()
	{
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		Debug.Log("CutsceneComplete :: HandleOnObjectiveComplete", this);
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
			if (this.m_ActiveSetter != null)
			{
				this.m_ActiveSetter.SetActive(active: false);
			}
		}
		return flag;
	}

	private void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
