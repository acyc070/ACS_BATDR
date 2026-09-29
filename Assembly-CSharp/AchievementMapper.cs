using System.Collections.Generic;
using UnityEngine;

public class AchievementMapper : MonoBehaviour
{
	[SerializeField]
	private List<AchievementIDMapping> achievementIDs;

	public AchievementIDMapping[] AchievementIDs => this.achievementIDs.ToArray();
}
