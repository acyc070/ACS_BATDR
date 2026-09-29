using UnityEngine;

public class DOTweenData
{
	[SerializeField]
	private bool m_IsPlaying;

	[SerializeField]
	private float m_AnimationTime;

	public bool IsPlaying => this.m_IsPlaying;

	public float AnimationTime => this.m_AnimationTime;

	public void Set(float time)
	{
		this.m_IsPlaying = true;
		this.m_AnimationTime = time;
	}
}
