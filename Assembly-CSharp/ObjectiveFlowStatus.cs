using UnityEngine;

public class ObjectiveFlowStatus : Objective
{
	[SerializeField]
	private bool m_IsDisabled;

	protected override void InternalInitialize()
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			if (this.m_IsDisabled)
			{
				GameManager.Instance.DisableTeleport();
				GameManager.Instance.Player.LockAbilities();
			}
			else
			{
				GameManager.Instance.EnableTeleport();
				GameManager.Instance.Player.UnlockAbilities();
			}
		}
		base.SendOnComplete();
	}
}
