using System;

[Serializable]
public class WeaponData
{
	public WeaponType WeaponType = WeaponType.LEVEL_1;

	public WeaponData()
	{
	}

	public WeaponData(WeaponType _type)
	{
		this.WeaponType = _type;
	}
}
