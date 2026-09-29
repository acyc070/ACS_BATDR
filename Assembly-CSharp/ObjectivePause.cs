using UnityEngine;

public class ObjectivePause : Objective
{
	[SerializeField]
	private bool m_Unlock = true;

	protected override void InternalInitialize()
	{
		if (this.m_Unlock)
		{
			GameManager.Instance.UnlockPause();
		}
		else
		{
			GameManager.Instance.LockPause();
		}
		base.SendOnComplete();
	}
}
