using UnityEngine;

public class VisibleChecker : JComponent
{
	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private bool m_IsSingle = true;

	[SerializeField]
	private Renderer m_VisibleCheck;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private bool m_IsActivated;

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	private void Update()
	{
		if (this.m_IsActive && (!this.m_IsSingle || !this.m_IsActivated) && !this.m_VisibleCheck.isVisible)
		{
			if (this.m_IsSingle)
			{
				this.m_IsActivated = true;
			}
			if (this.m_ActiveSetter != null)
			{
				this.m_ActiveSetter.SetActive(active: true);
			}
		}
	}
}
