using UnityEngine;
using UnityEngine.Events;

public class AnimationCompleteEvent : JMonoBehaviour
{
	[SerializeField]
	private UnityEvent m_UnityEvent;

	public void AnimationComplete()
	{
		this.m_UnityEvent?.Invoke();
	}
}
