using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AsyncLoaderController : UIController
{
	[Header("Image")]
	[SerializeField]
	private Image m_Loader;

	[Header("Frames")]
	[SerializeField]
	private Sprite[] m_Fames;

	private int m_FrameIndex;

	private bool m_IsLoading;

	protected override void OnInitialized(object _data)
	{
		this.m_Loader.color = new Color(1f, 1f, 1f, 0f);
		this.m_FrameIndex = 0;
		this.m_Loader.sprite = this.m_Fames[this.m_FrameIndex];
		this.m_IsLoading = true;
		base.StartCoroutine(this.DOLoader());
	}

	private IEnumerator DOLoader()
	{
		this.m_Loader.color = new Color(1f, 1f, 1f, 1f);
		this.m_Loader.sprite = this.m_Fames[0];
		unchecked
		{
			while (this.m_IsLoading)
			{
				this.m_Loader.sprite = this.m_Fames[this.m_FrameIndex];
				this.m_FrameIndex--;
				if (this.m_FrameIndex < 0)
				{
					this.m_FrameIndex = this.m_Fames.Length - 1;
				}
				yield return new WaitForSecondsRealtime(0.015f);
			}
			this.m_Loader.color = new Color(1f, 1f, 1f, 0f);
			this.PlayOut();
		}
	}

	public void LoadingComplete()
	{
		this.m_FrameIndex = 0;
		this.m_IsLoading = false;
	}

	protected override void OnDisposed()
	{
		base.StopCoroutine(this.DOLoader());
		base.OnDisposed();
	}
}
