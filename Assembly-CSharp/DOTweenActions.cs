using DG.Tweening;
using UnityEngine;

public class DOTweenActions : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation[] m_DOTweens;

	[SerializeField]
	private Transform[] m_Transforms;

	private Sequence m_Sequence;

	public void DOPlay()
	{
		unchecked
		{
			if (this.m_Transforms != null && this.m_Transforms.Length != 0)
			{
				this.m_Sequence?.Kill();
				this.m_Sequence = DOTween.Sequence();
				for (int i = 0; i < this.m_Transforms.Length; i++)
				{
					TweenSettingsExtensions.Insert(t: this.m_Transforms[i].DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.InOutSine), s: this.m_Sequence, atPosition: 0f);
				}
			}
			else if (this.m_DOTweens != null && this.m_DOTweens.Length != 0)
			{
				for (int j = 0; j < this.m_DOTweens.Length; j++)
				{
					this.m_DOTweens[j].DOPlay();
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
		if (this.m_DOTweens != null)
		{
			for (int i = 0; i < this.m_DOTweens.Length; i = unchecked(i + 1))
			{
				this.m_DOTweens[i].DOKill();
			}
		}
		base.OnDisposed();
	}
}
