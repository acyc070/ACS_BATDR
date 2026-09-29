using System;
using DG.Tweening;
using UnityEngine;

public class GhostSubwayController : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Content")]
	[SerializeField]
	private GameObject m_Content;

	[Header("Subway Car")]
	[SerializeField]
	private Transform m_SubwayCar;

	[Header("Locations")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[Space]
	[SerializeField]
	private Transform m_AudioStartLocation;

	[SerializeField]
	private Transform m_AudioEndLocation;

	[Header("Audio")]
	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioSource m_AudioSourceLoop;

	private Sequence m_Sequence;

	private bool m_IsInitialized;

	private bool m_IsActivated;

	public override void Start()
	{
		this.m_Content.SetActive(value: false);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void Update()
	{
		if (this.m_IsInitialized && !this.m_IsActivated && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			DateTime now;
			now = DateTime.Now;
			if ((now.Hour == 4 || now.Hour == 16) && now.Minute == 14)
			{
				this.Execute();
			}
		}
	}

	private void Execute()
	{
		this.m_IsActivated = true;
		this.m_AudioSourceLoop.Stop();
		this.m_AudioSource.Stop();
		this.m_SubwayCar.position = this.m_StartLocation.position;
		this.m_Sequence.Kill();
		this.m_Sequence = DOTween.Sequence();
		this.m_Content.SetActive(value: true);
		this.m_AudioSourceLoop.Play();
		this.m_AudioSource.transform.position = this.m_AudioStartLocation.position;
		this.m_Sequence.InsertCallback(4f, this.m_AudioSource.Play);
		this.m_Sequence.Insert(5f, this.m_AudioSource.transform.DOMove(this.m_AudioEndLocation.position, 4f).SetEase(Ease.OutSine));
		this.m_Sequence.Insert(5f, this.m_SubwayCar.DOMove(this.m_EndLocation.position, 4f).SetEase(Ease.OutSine));
		this.m_Sequence.InsertCallback(11f, () =>
		{
			this.m_AudioSourceLoop.Stop();
			this.m_Content.SetActive(value: false);
		});
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("GhostSubwayController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
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
			this.m_IsInitialized = true;
		}
		return flag;
	}

	protected override void OnDisposed()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
		base.OnDisposed();
	}
}
