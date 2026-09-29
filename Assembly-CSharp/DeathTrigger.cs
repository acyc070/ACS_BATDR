using UnityEngine;

public class DeathTrigger : EventTrigger
{
	protected override void OnInternalEnter(Collider col)
	{
		GameManager.Instance.Player.ForceDeath();
		base.ResetAction();
	}
}
