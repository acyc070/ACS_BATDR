using DG.Tweening;
using UnityEngine;

public class DummyDeath : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private Sequence m_Sequence;

	public override void OnEnable()
	{
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0, 360), 0f);
		this.m_Content.localScale = Vector3.one;
		this.ResetSequence();
		this.m_Sequence.Insert(5f, this.m_Content.DOScale(0f, 2f).SetEase(Ease.InSine));
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
