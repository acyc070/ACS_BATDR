using DG.Tweening;
using UnityEngine;

public class DOTweenActiveChecker : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation m_DOTweenAnimation;

	public override void OnEnable()
	{
		if (!(this.m_DOTweenAnimation == null) && this.m_DOTweenAnimation.autoPlay)
		{
			this.m_DOTweenAnimation.DOPlay();
		}
	}

	public override void OnDisable()
	{
		if (!(this.m_DOTweenAnimation == null) && this.m_DOTweenAnimation.autoPlay)
		{
			this.m_DOTweenAnimation.DOPause();
		}
	}
}
