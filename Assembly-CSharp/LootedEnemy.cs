using DG.Tweening;
using UnityEngine;

public class LootedEnemy : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private Sequence m_Sequence;

	public void Initialize(Vector3 toPosition)
	{
		this.m_Content.localScale = Vector3.one * Random.Range(0.9f, 1.03f);
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		if (Physics.Linecast(toPosition, toPosition + Vector3.down * 10f, out var hitInfo, ~(1 << LayerMask.NameToLayer("IgnorePlayer")), QueryTriggerInteraction.Ignore))
		{
			toPosition = hitInfo.point;
		}
		base.transform.position = toPosition;
		this.ResetSequence();
		this.m_Sequence.Insert(6f, this.m_Content.DOScale(0f, 3f).SetEase(Ease.InSine));
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
