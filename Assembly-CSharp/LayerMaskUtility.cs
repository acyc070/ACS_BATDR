using UnityEngine;

public static class LayerMaskUtility
{
	public static int GetWeaponAttack => ~(LayerMaskUtility.Player() | LayerMaskUtility.InvisibleCollider() | LayerMaskUtility.InteractInvisibleCollider());

	public static int GetInvisibleColliders => ~(LayerMaskUtility.Player() | LayerMaskUtility.IgnorePlayer() | LayerMaskUtility.InvisibleCollider() | LayerMaskUtility.InteractInvisibleCollider());

	public static int Player()
	{
		return 1 << LayerMask.NameToLayer("Player");
	}

	public static int IgnorePlayer()
	{
		return 1 << LayerMask.NameToLayer("IgnorePlayer");
	}

	public static int InvisibleCollider()
	{
		return 1 << LayerMask.NameToLayer("InvisibleCollider");
	}

	public static int InteractInvisibleCollider()
	{
		return 1 << LayerMask.NameToLayer("InteractInvisibleCollider");
	}

	public static int Enemy()
	{
		return 1 << LayerMask.NameToLayer("AI");
	}

	public static int IgnoreEnemy()
	{
		return 1 << LayerMask.NameToLayer("IgnoreAI");
	}
}
