using UnityEngine;

public class ObjectiveNavigation : Objective
{
	[SerializeField]
	protected bool m_IsReal;

	[SerializeField]
	private Transform m_Destination;

	[SerializeField]
	private bool m_IsRanged;

	[SerializeField]
	private float m_Range = 10f;

	private bool m_IsInRange;

	private bool m_IsActiveRange;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.ShowNavigation();
	}

	protected override void InternalDisable()
	{
		base.Inactive();
	}

	protected override void InternalInactive()
	{
		this.ClearNavigation();
	}

	private void ShowNavigation()
	{
		if (this.m_Destination != null)
		{
			if (this.m_IsRanged)
			{
				this.m_IsActiveRange = true;
			}
			else
			{
				GameManager.Instance.ShowNavigation(this.m_Destination, this.m_IsReal);
			}
		}
	}

	private void ClearNavigation()
	{
		GameManager.Instance.ClearNavigation();
		this.m_IsInRange = false;
		this.m_IsActiveRange = false;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		base.SendOnComplete();
	}

	protected override void InternalUpdateComplete()
	{
		if (this.m_IsActiveRange && this.m_IsRanged && this.m_Destination != null && GameManager.Instance.Player != null)
		{
			float num;
			num = Vector3.Distance(GameManager.Instance.Player.transform.position, this.m_Destination.position);
			if (!this.m_IsInRange && num <= this.m_Range)
			{
				GameManager.Instance.ShowNavigation(this.m_Destination, this.m_IsReal);
				this.m_IsInRange = true;
			}
			else if (this.m_IsInRange && num > this.m_Range)
			{
				GameManager.Instance.ClearNavigation();
				this.m_IsInRange = false;
			}
		}
	}
}
