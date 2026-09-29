using DG.Tweening;
using UnityEngine;

public class InteractableProjector : InteractableInputDisplay
{
	[Header("Reel Options")]
	[SerializeField]
	private Transform[] m_Reels;

	public bool IsOn { get; private set; }

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		this.IsOn = !this.IsOn;
		if (this.IsOn)
		{
			this.TurnReels();
		}
		else
		{
			this.StopReels();
		}
	}

	public void TurnReels()
	{
		base.SetInteractionType(InteractionType.INTERACTION_TURN_OFF);
		float num;
		num = -360f;
		for (int i = 0; i < this.m_Reels.Length; i = unchecked(i + 1))
		{
			Transform target;
			target = this.m_Reels[i];
			target.DOKill();
			target.DOLocalRotate(new Vector3(num, 0f, 0f), 2f, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
			num *= -1f;
		}
	}

	private void StopReels()
	{
		base.SetInteractionType(InteractionType.INTERACTION_TURN_ON);
		this.KillReelTweens();
	}

	private void KillReelTweens()
	{
		for (int i = 0; i < this.m_Reels.Length; i = unchecked(i + 1))
		{
			this.m_Reels[i].DOKill();
		}
	}

	protected override void OnDisposed()
	{
		this.KillReelTweens();
		base.OnDisposed();
	}
}
