using UnityEngine;

public class AutoLoopAnimation : JMonoBehaviour
{
	[SerializeField]
	private Animator m_Animator;

	public void AnimationComplete()
	{
		this.m_Animator.ResetTrigger("ForceReset");
		this.m_Animator.SetTrigger("ForceReset");
	}
}
