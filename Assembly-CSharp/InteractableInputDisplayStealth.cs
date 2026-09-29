using UnityEngine;

public class InteractableInputDisplayStealth : InteractableInputDisplay
{
	protected override bool InternalEnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth)
		{
			return false;
		}
		return base.InternalEnterCheck(origin, hit, sender);
	}

	protected override bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth)
		{
			return false;
		}
		return base.InternalInteractCheck(origin, hit, sender);
	}
}
