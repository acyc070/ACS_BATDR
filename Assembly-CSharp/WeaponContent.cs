using UnityEngine;

public class WeaponContent : ItemContent<Weapon, WeaponData>
{
	public override void Initialize(object data)
	{
		base.Initialize(data);
		base.SetLayerRecursive(LayerMask.NameToLayer("FirstPerson"));
	}
}
