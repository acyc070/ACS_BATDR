using System;
using DG.Tweening;
using UnityEngine;

public class CitySubwayCarController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_SubwayCar;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private Transform m_AudioLocation;

	private Sequence m_Sequence;

	private float m_Timer;

	private float m_TimerRate = 30f;

	public event EventHandler OnTrigger;

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.m_Timer += Time.deltaTime;
			if (this.m_Timer > this.m_TimerRate)
			{
				this.m_Timer = 0f;
				this.Trigger();
			}
		}
	}

	private void Trigger()
	{
		this.OnTrigger.Send(this);
		this.m_SubwayCar.localPosition = this.m_StartLocation.localPosition;
		this.m_SubwayCar.gameObject.SetActive(value: true);
		this.ResetSequence();
		this.m_Sequence.Insert(0.5f, this.m_SubwayCar.DOLocalMove(this.m_EndLocation.localPosition, 1.5f).SetEase(Ease.Linear));
		this.m_Sequence.InsertCallback(0.6f, () =>
		{
			Vector3 position;
			position = this.m_AudioLocation.position;
			position.y = GameManager.Instance.Player.transform.position.y;
			if (Vector3.Distance(GameManager.Instance.Player.transform.position, position) < 25f)
			{
				CameraEffects.ShakeRotation(2f, 1f);
			}
		});
		this.m_Sequence.OnComplete(this.Deactivate);
	}

	private void Deactivate()
	{
		this.m_SubwayCar.gameObject.SetActive(value: false);
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
