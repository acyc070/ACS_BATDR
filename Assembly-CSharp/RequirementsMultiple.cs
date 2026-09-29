using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Requirements/New Requirements Multiple")]
public class RequirementsMultiple : Requirements
{
	[SerializeField]
	private Requirement[] m_SecondaryList;

	public Requirement[] SecondaryList => this.m_SecondaryList;

	protected override bool InternalIsComplete(bool isComplete)
	{
		bool flag;
		flag = isComplete;
		if (!flag)
		{
			flag = true;
			for (int i = 0; i < this.m_SecondaryList.Length; i = unchecked(i + 1))
			{
				Requirement requirement;
				requirement = this.m_SecondaryList[i];
				if (requirement != null)
				{
					if (!requirement.IsComplete())
					{
						flag = false;
						break;
					}
					continue;
				}
				flag = false;
				break;
			}
		}
		return flag;
	}
}
