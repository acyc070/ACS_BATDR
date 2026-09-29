using DG.Tweening;
using UnityEngine;

public class DOTweenPlayOnActive : JMonoBehaviour
{
	[SerializeField]
	private DOTweenAnimation m_Animation;

	public override void OnEnable()
	{
		this.m_Animation.DOPlay();
	}

	public override void OnDisable()
	{
		this.m_Animation.DORestart();
		this.m_Animation.DOPause();
	}
}
