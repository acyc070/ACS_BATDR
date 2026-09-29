using System;
using DG.Tweening;
using UnityEngine;

public class UIPostCredits : UIController
{
	[SerializeField]
	private VideoController m_Video;

	protected override void OnInitialized(object _data)
	{
		this.m_Video.SetCamera(GameManager.Instance.UIManager.Camera);
	}

	public override void PlayIn()
	{
		DOTween.Sequence().InsertCallback(0.5f, () =>
		{
			this.m_Video.Play();
			this.m_Video.OnComplete -= this.HandleVideoOnComplete;
			this.m_Video.OnComplete += this.HandleVideoOnComplete;
			this.PlayInComplete();
		});
	}

	private void HandleVideoOnComplete(object sender, EventArgs e)
	{
		this.m_Video.OnComplete -= this.HandleVideoOnComplete;
		DOTween.Sequence().InsertCallback(1f, () =>
		{
			GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW").OnPlayInComplete += this.HandleUICreditsOnPlayInComplete;
		});
	}

	private void HandleUICreditsOnPlayInComplete(object sender, EventArgs e)
	{
		(sender as UICredits).OnPlayInComplete -= this.HandleUICreditsOnPlayInComplete;
		this.PlayOut();
	}

	protected override void OnDisposed()
	{
		this.m_Video.OnComplete -= this.HandleVideoOnComplete;
		base.OnDisposed();
	}
}
