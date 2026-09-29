using UnityEngine;

public class EventTriggerMulti : EventTrigger
{
	[Header("Multi Trigger Tags")]
	[SerializeField]
	[TagSelector]
	private string[] m_TriggerTags;

	private int m_ActiveTriggers;

	protected override bool InternalEnterCheck(Collider col)
	{
		unchecked
		{
			if (this.CheckTag(col.tag))
			{
				this.m_ActiveTriggers++;
				if (this.m_ActiveTriggers == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}

	protected override bool InternalExitCheck(Collider col)
	{
		unchecked
		{
			if (this.CheckTag(col.tag))
			{
				this.m_ActiveTriggers--;
				if (this.m_ActiveTriggers <= 0)
				{
					this.m_ActiveTriggers = 0;
					return true;
				}
				return false;
			}
			return false;
		}
	}

	protected override bool CanTrigger(bool _isTriggered, Collider col)
	{
		return true;
	}

	protected override bool CheckTag(string tag)
	{
		bool result;
		result = false;
		for (int i = 0; i < this.m_TriggerTags.Length; i = unchecked(i + 1))
		{
			if (this.m_TriggerTags[i] == tag)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
