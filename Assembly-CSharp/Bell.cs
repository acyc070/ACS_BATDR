using System;
using DG.Tweening;
using UnityEngine;

public class Bell : JMonoBehaviour
{
	[SerializeField]
	private Transform m_BellTop;

	[SerializeField]
	private Interactable m_Interactable;

	private Sequence m_Sequence;

	public event EventHandler OnRing;

	public override void Start()
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_BellTop.DOLocalMoveY(-0.03f, 0.05f).SetEase(Ease.Linear));
		this.m_Sequence.Insert(0.05f, this.m_BellTop.DOLocalMoveY(0f, 0.1f).SetEase(Ease.OutBounce));
		this.OnRing.Send(this);
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
		this.OnRing = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
