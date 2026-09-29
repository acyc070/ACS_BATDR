using UnityEngine;

public class AnimationCycler : JMonoBehaviour
{
	[SerializeField]
	private float m_TimerLimit = 5f;

	[SerializeField]
	private CharacterContent m_Content;

	[SerializeField]
	private AnimationClip[] m_AnimationClips;

	private bool m_IsActive;

	private float m_Timer;

	private int m_Index;

	private bool m_IsDisabled;

	public override void OnEnable()
	{
		this.SetActive(active: true);
	}

	public override void OnDisable()
	{
		this.SetActive(active: false);
	}

	private void Update()
	{
		if (!this.m_IsActive || this.m_IsDisabled || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Timer >= this.m_TimerLimit)
		{
			if (this.m_IsActive)
			{
				this.m_IsActive = false;
				this.m_Timer = 0f;
				AnimationClip animationClip;
				animationClip = this.m_AnimationClips[this.m_Index];
				animationClip.name = "Interact";
				this.m_Content.UpdateClipOverride(animationClip);
				this.m_Content.SetAnimationTrigger("Interact");
			}
		}
		else
		{
			this.m_Timer += Time.deltaTime;
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public void Disable()
	{
		this.SetActive(active: false);
		this.m_IsDisabled = true;
	}

	public void AnimationComplete()
	{
		unchecked
		{
			if (!this.m_IsDisabled)
			{
				this.m_Index++;
				if (this.m_Index >= this.m_AnimationClips.Length)
				{
					this.m_Index = 0;
				}
				if (base.gameObject.activeSelf)
				{
					this.m_IsActive = true;
				}
				this.m_Timer = 0f;
			}
		}
	}
}
