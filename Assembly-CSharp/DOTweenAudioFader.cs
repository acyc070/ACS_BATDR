using DG.Tweening;
using UnityEngine;

public class DOTweenAudioFader : JMonoBehaviour
{
	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private float m_StartVolume;

	[SerializeField]
	private float m_EndVolume;

	[SerializeField]
	private float m_Duration;

	public override void Start()
	{
		this.m_AudioSource.volume = this.m_StartVolume;
		this.m_AudioSource.DOKill();
		this.m_AudioSource.DOFade(this.m_EndVolume, this.m_Duration).SetEase(Ease.Linear);
	}
}
