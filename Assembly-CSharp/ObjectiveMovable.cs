using UnityEngine;

public class ObjectiveMovable : Objective
{
	[SerializeField]
	private Movable m_Movable;

	protected override void InternalInitialize()
	{
		this.CheckState(enable: true);
	}

	protected override void InternalEnable()
	{
		this.CheckState(enable: true);
	}

	protected override void InternalDisable()
	{
		this.CheckState(enable: false);
	}

	protected override void InternalInactive()
	{
		this.CheckState(enable: false);
	}

	private void CheckState(bool enable)
	{
		if (!(this.m_Movable == null))
		{
			if (enable)
			{
				this.m_Movable.Content.Enable();
			}
			else
			{
				this.m_Movable.Content.Disable();
			}
		}
	}
}
