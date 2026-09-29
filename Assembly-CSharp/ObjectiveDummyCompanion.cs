using System;
using DG.Tweening;
using UnityEngine;

public class ObjectiveDummyCompanion : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Dummy Stuff")]
	[SerializeField]
	private Dummy m_DummyCompanion;

	[SerializeField]
	private Transform m_ToLocation;

	private bool m_IsChecking;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	protected override void InternalUpdate()
	{
		if (this.m_IsChecking && Vector3.Distance(this.m_DummyCompanion.transform.position, this.m_ToLocation.position) < 1.5f)
		{
			this.m_IsChecking = false;
			Sequence sequence;
			sequence = DOTween.Sequence();
			sequence.Insert(0f, this.m_DummyCompanion.transform.DOMove(this.m_ToLocation.position, 0.25f).SetEase(Ease.Linear));
			sequence.Insert(0f, this.m_DummyCompanion.transform.DORotate(this.m_ToLocation.eulerAngles, 0.25f).SetEase(Ease.Linear));
			sequence.OnComplete(this.SequenceOnComplete);
		}
	}

	private void SequenceOnComplete()
	{
		this.m_DummyCompanion.gameObject.SetActive(value: false);
		base.SendOnComplete();
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
			GameManager.Instance.DummyManager.RemoveDummiesOf(this.m_DummyCompanion.transform);
			this.m_DummyCompanion.SetStopDistance(0f);
			this.m_DummyCompanion.SetTarget(this.m_ToLocation);
			this.m_IsChecking = true;
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}
}
