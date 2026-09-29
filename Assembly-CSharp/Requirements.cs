using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Requirements/New Requirements")]
public class Requirements : ScriptableObject
{
	[SerializeField]
	protected Requirement[] m_List;

	public Requirement[] List => this.m_List;

	public bool IsComplete()
	{
		bool isComplete;
		isComplete = true;
		for (int i = 0; i < this.m_List.Length; i = unchecked(i + 1))
		{
			Requirement requirement;
			requirement = this.m_List[i];
			if (requirement != null)
			{
				if (!requirement.IsComplete())
				{
					isComplete = false;
					break;
				}
				continue;
			}
			isComplete = false;
			break;
		}
		return this.InternalIsComplete(isComplete);
	}

	protected virtual bool InternalIsComplete(bool isComplete)
	{
		return isComplete;
	}
}
