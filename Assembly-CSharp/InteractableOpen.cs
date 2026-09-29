using DG.Tweening;
using UnityEngine;

public class InteractableOpen : InteractableInputDisplay
{
	[Header("Open Options")]
	[SerializeField]
	private Transform m_Pivot;

	[SerializeField]
	private Vector3 m_OpenPivot;

	[SerializeField]
	private float m_Speed = 1f;

	[SerializeField]
	private Ease m_OpenEase;

	private Sequence m_Sequence;

	private Vector3 m_ClosedPivot;

	public override void Awake()
	{
		base.Awake();
		this.m_ClosedPivot = this.m_Pivot.localEulerAngles;
	}

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		this.m_Sequence?.Kill();
		this.m_Sequence = DOTween.Sequence();
		this.m_Sequence.Insert(0f, this.m_Pivot.DOLocalRotate(this.m_OpenPivot, this.m_Speed, RotateMode.LocalAxisAdd).SetEase(this.m_OpenEase));
	}

	protected override void OnDisposed()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
		base.OnDisposed();
	}
}
