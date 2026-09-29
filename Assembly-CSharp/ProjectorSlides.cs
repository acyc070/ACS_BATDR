using System;
using DG.Tweening;
using UnityEngine;

public class ProjectorSlides : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ProjectorID;

	[Header("Index")]
	[SerializeField]
	private int m_ID = 3;

	[Space]
	[SerializeField]
	private Interactable m_Interactable;

	[Space]
	[SerializeField]
	private GameObject m_Lights;

	[Header("Sprite Renderer")]
	[SerializeField]
	private SpriteRenderer m_Slide;

	[Header("Sprites")]
	[SerializeField]
	private Sprite[] m_Slides;

	[Header("Reels")]
	[SerializeField]
	private Transform m_BackReel;

	[SerializeField]
	private Transform m_FrontReel;

	private Sequence m_Sequence;

	private bool m_Isnteracted;

	public int ID => this.m_ProjectorID;

	public int Index => this.m_ID;

	public void Initialize()
	{
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		if (!this.m_Isnteracted)
		{
			this.NextSlide();
		}
	}

	private void NextSlide()
	{
		this.Disable();
		unchecked
		{
			this.m_ID++;
			if (this.m_ID > 5)
			{
				this.m_ID = 1;
			}
			this.m_Slide.sprite = this.m_Slides[this.m_ID - 1];
			this.ResetSequence();
			this.m_Sequence.Insert(0f, this.m_BackReel.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.InBack));
			this.m_Sequence.Insert(0f, this.m_FrontReel.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InBack));
			this.m_Sequence.OnComplete(this.Enable);
		}
	}

	private void Disable()
	{
		this.m_Isnteracted = true;
		this.m_Lights.SetActive(value: false);
	}

	private void Enable()
	{
		this.m_Isnteracted = false;
		this.m_Lights.SetActive(value: true);
	}

	public void SetIndex(int index)
	{
		this.m_ID = index;
		if (this.m_ID > 5)
		{
			this.m_ID = 1;
		}
		this.m_Slide.sprite = this.m_Slides[unchecked(this.m_ID - 1)];
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
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		}
		base.OnDisposed();
	}
}
