using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBlocker : UIController
{
	[Header("Canvas Groups")]
	[SerializeField]
	private CanvasGroup m_Blocker;

	[SerializeField]
	private Image m_Image;

	private bool m_IsIndependent;

	public event EventHandler OnShow;

	public event EventHandler OnHide;

	protected override void OnInitialized(object _data)
	{
		this.m_Blocker.alpha = 0f;
		this.m_Blocker.gameObject.SetActive(value: false);
	}

	public bool Show(float duration = 0.5f, float delay = 0f, bool isWhite = false)
	{
		if (this.m_Blocker.alpha >= 1f)
		{
			return false;
		}
		if (isWhite)
		{
			this.m_Image.color = Color.white;
		}
		else
		{
			this.m_Image.color = Color.black;
		}
		this.DOFade(1f, duration, delay).OnComplete(() =>
		{
			this.OnShow.Send(this);
		});
		return true;
	}

	public bool Hide(float duration = 0.5f, float delay = 0f, bool isWhite = false)
	{
		if (this.m_Blocker.alpha <= 0f)
		{
			return false;
		}
		if (isWhite)
		{
			this.m_Image.color = Color.white;
		}
		else
		{
			this.m_Image.color = Color.black;
		}
		this.DOFade(0f, duration, delay).OnComplete(this.DisableBlocker);
		return true;
	}

	private Tweener DOFade(float alpha, float duration, float delay)
	{
		this.m_Blocker.DOKill();
		this.m_Blocker.gameObject.SetActive(value: true);
		return this.m_Blocker.DOFade(alpha, duration).SetDelay(delay).SetEase(Ease.Linear)
			.SetUpdate(UpdateType.Normal, this.m_IsIndependent);
	}

	public void SetIndependent(bool isIndependent)
	{
		this.m_IsIndependent = isIndependent;
	}

	private void DisableBlocker()
	{
		this.m_Blocker.gameObject.SetActive(value: false);
		this.OnHide.Send(this);
		this.PlayOut();
	}

	protected override void OnDisposed()
	{
		this.m_Blocker.DOKill();
		this.OnShow = null;
		this.OnHide = null;
		base.OnDisposed();
	}
}
