using UnityEngine;

public static class PrefabCheck
{
	public static string GetEnemy(EnemyType enemyType)
	{
		string result;
		result = string.Empty;
		switch (enemyType)
		{
		case EnemyType.LostOne:
			result = "Enemies/Enemy_LostOne";
			break;
		case EnemyType.LostOneColor:
			result = "Enemies/Enemy_LostOne_Color";
			break;
		case EnemyType.Searcher:
			result = ((Random.value < 0.5f) ? "Enemies/Enemy_Searcher_01" : "Enemies/Enemy_Searcher_02");
			break;
		case EnemyType.Keeper:
			result = "Enemies/Enemy_Keeper";
			break;
		case EnemyType.ButcherGang_Fisher:
			result = "Enemies/Enemy_ButcherGang_Fisher";
			break;
		case EnemyType.ButcherGang_Piper:
			result = "Enemies/Enemy_ButcherGang_Piper";
			break;
		case EnemyType.ButcherGang_Striker:
			result = "Enemies/Enemy_ButcherGang_Striker";
			break;
		}
		return result;
	}

	public static string GetCharacter(EnemyType enemyType, LostOneType lostOneType = LostOneType.NONE)
	{
		string result;
		result = string.Empty;
		switch (enemyType)
		{
		case EnemyType.LostOne:
		case EnemyType.LostOneColor:
			result = PrefabCheck.GetLostOne(lostOneType);
			break;
		case EnemyType.Searcher:
			result = ((Random.value < 0.5f) ? "Characters/Prefab_Character_Searcher" : "Characters/Prefab_Character_Searcher_02");
			break;
		case EnemyType.KingWidow:
			result = "Characters/Prefab_Character_KingWidow";
			break;
		case EnemyType.Keeper:
			result = "Characters/Prefab_Character_Keeper_Hover";
			break;
		}
		return result;
	}

	public static string GetLostOne(LostOneType lostOneType)
	{
		string result;
		result = string.Empty;
		switch (lostOneType)
		{
		case LostOneType.Female:
			result = "Characters/Prefab_Character_LostOneFemale";
			break;
		case LostOneType.Female_Amok_Follower:
			result = "Characters/Prefab_Character_LostOneFemale_Amok_Follower";
			break;
		case LostOneType.Female_Amok_Follower_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_Amok_Follower_Vicious";
			break;
		case LostOneType.Female_BagHead:
			result = "Characters/Prefab_Character_LostOneFemale_BagHead";
			break;
		case LostOneType.Female_BagHead_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_BagHead_Vicious";
			break;
		case LostOneType.Female_Color:
			result = "Characters/Prefab_Character_LostOneFemale_Color";
			break;
		case LostOneType.Female_Color_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_Color_Vicious";
			break;
		case LostOneType.Female_Hambush:
			result = "Characters/Prefab_Character_LostOneFemale_Hambush";
			break;
		case LostOneType.Female_Hambush_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_Hambush_Vicious";
			break;
		case LostOneType.Female_Melanie:
			result = "Characters/Prefab_Character_LostOneFemale_Melanie";
			break;
		case LostOneType.Female_Melanie_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_Melanie_Vicious";
			break;
		case LostOneType.Female_Vicious:
			result = "Characters/Prefab_Character_LostOneFemale_Vicious";
			break;
		case LostOneType.Male:
			result = "Characters/Prefab_Character_LostOneMale";
			break;
		case LostOneType.Male_Amok_Follower:
			result = "Characters/Prefab_Character_LostOneMale_Amok_Follower";
			break;
		case LostOneType.Male_Amok_Follower_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Amok_Follower_Vicious";
			break;
		case LostOneType.Male_BagHead:
			result = "Characters/Prefab_Character_LostOneMale_BagHead";
			break;
		case LostOneType.Male_BagHead_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_BagHead_Vicious";
			break;
		case LostOneType.Male_Color:
			result = "Characters/Prefab_Character_LostOneMale_Color";
			break;
		case LostOneType.Male_Color_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Color_Vicious";
			break;
		case LostOneType.Male_Hal:
			result = "Characters/Prefab_Character_LostOneMale_Hal";
			break;
		case LostOneType.Male_Hal_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Hal_Vicious";
			break;
		case LostOneType.Male_Hambush:
			result = "Characters/Prefab_Character_LostOneMale_Hambush";
			break;
		case LostOneType.Male_Hambush_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Hambush_Vicious";
			break;
		case LostOneType.Male_Matt:
			result = "Characters/Prefab_Character_LostOneMale_Matt";
			break;
		case LostOneType.Male_Matt_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Matt";
			break;
		case LostOneType.Male_Scott:
			result = "Characters/Prefab_Character_LostOneMale_Scott";
			break;
		case LostOneType.Male_Scott_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Scott";
			break;
		case LostOneType.Male_Vicious:
			result = "Characters/Prefab_Character_LostOneMale_Vicious";
			break;
		case LostOneType.Male_Amok:
			result = "Characters/Prefab_Character_LostOneMale_Amok";
			break;
		}
		return result;
	}

	public static string GetButcherGange(EnemyType enemyType)
	{
		return PrefabCheck.GetEnemy(enemyType);
	}
}
