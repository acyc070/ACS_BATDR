using System;
using DG.Tweening;
using UnityEngine;

public class SimpleDOTweenTo : MonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private GameObject m_TweenGameObject;

	[SerializeField]
	private Transform m_ToTween;

	[SerializeField]
	private float m_Speed = 3f;

	private void Start()
	{
		this.m_EventTrigger.OnEnter += this.M_EventTrigger_OnEnter;
		this.m_TweenGameObject.SetActive(value: false);
	}

	private void M_EventTrigger_OnEnter(object sender, EventArgs e)
	{
		this.m_TweenGameObject.SetActive(value: true);
		this.m_TweenGameObject.transform.DOMove(this.m_ToTween.position, this.m_Speed).SetEase(Ease.Linear);
		this.m_TweenGameObject.transform.DORotate(this.m_ToTween.eulerAngles, this.m_Speed).SetEase(Ease.Linear).OnComplete(this.onComplete);
	}

	private void onComplete()
	{
		this.m_TweenGameObject.SetActive(value: false);
	}
}
