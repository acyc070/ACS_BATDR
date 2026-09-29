using System;
using DG.Tweening;
using UnityEngine;

public class PuzzleWheel : JMonoBehaviour
{
	private static Vector3 WHEEL_ROTATION;

	[SerializeField]
	private Interactable m_Interactable;

	private Sequence m_Sequence;

	public int ID { get; private set; }

	public void Initialize(int id)
	{
		this.ID = id;
		while (this.ID == id)
		{
			this.ID = global::UnityEngine.Random.Range(1, 6);
		}
		if (this.ID > 1)
		{
			this.m_Interactable.transform.localEulerAngles += PuzzleWheel.WHEEL_ROTATION * unchecked(this.ID - 1);
		}
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		GameManager.Instance.Player.Interaction.ResetInteraction();
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_Interactable.transform.DOLocalRotate(PuzzleWheel.WHEEL_ROTATION, 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack));
		this.m_Sequence.OnComplete(this.WheelOnComplete);
	}

	private void WheelOnComplete()
	{
		unchecked
		{
			this.ID++;
			if (this.ID > 6)
			{
				this.ID = 1;
			}
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
			this.m_Interactable.ResetAction();
			this.m_Interactable.SetActive(active: true);
		}
	}

	public void SetActive(bool active)
	{
		this.m_Interactable.SetActive(active);
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

	static PuzzleWheel()
	{
		PuzzleWheel.WHEEL_ROTATION = new Vector3(-60f, 0f, 0f);
	}
}
