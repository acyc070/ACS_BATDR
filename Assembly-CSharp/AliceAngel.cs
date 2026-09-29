using UnityEngine;

public class AliceAngel : Hittable
{
	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.AssetManager.CreateAsset<Transform>("Impacts/Impact_Ink").position = hit.point;
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
		return true;
	}
}
