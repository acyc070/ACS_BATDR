using UnityEngine;

public class HeadTracker : JComponent
{
	[SerializeField]
	private bool m_IsActive = true;

	[Header("Transform Options")]
	[SerializeField]
	private Transform m_ForwardDirection;

	[SerializeField]
	private Vector3 m_ForwardOffset = new Vector3(0f, 270f, 270f);

	[SerializeField]
	private float m_Speed = 5f;

	[SerializeField]
	private float m_Distance = 8f;

	[SerializeField]
	private float m_Angle = 45f;

	[Header("Other Options")]
	[SerializeField]
	private bool m_IsInstantStart;

	private Transform m_Target;

	private Quaternion m_CurrentRotation;

	private Quaternion m_LastLookRotation;

	private bool m_InRange;

	private bool m_IgnoreTracking;

	public override void OnEnable()
	{
		this.m_LastLookRotation = base.transform.rotation;
		if (this.m_ForwardDirection == null)
		{
			this.m_ForwardDirection = base.transform;
		}
		if (this.m_IsActive && this.m_IsInstantStart)
		{
			this.InstantStart();
		}
	}

	private void InstantStart()
	{
		if (!(GameManager.Instance.Player == null))
		{
			if (this.m_Target == null)
			{
				this.m_Target = GameManager.Instance.Player.HeadContainer;
			}
			if (!(this.m_Target == null))
			{
				this.m_CurrentRotation = base.transform.rotation;
				this.m_InRange = Vector3.Distance(base.transform.position, this.m_Target.position) < this.m_Distance;
				Quaternion rotation;
				rotation = (this.CheckBasicFOV(this.m_ForwardDirection, this.m_Target, this.m_Angle) ? (Quaternion.LookRotation(this.m_Target.position - base.transform.position) * Quaternion.Euler(this.m_ForwardOffset)) : this.m_CurrentRotation);
				base.transform.rotation = rotation;
				this.m_LastLookRotation = base.transform.rotation;
			}
		}
	}

	private void LateUpdate()
	{
		if (this.m_IsActive && !(GameManager.Instance.Player == null))
		{
			if (this.m_Target == null)
			{
				this.m_Target = GameManager.Instance.Player.HeadContainer;
			}
			if (!(this.m_Target == null))
			{
				this.m_CurrentRotation = base.transform.rotation;
				this.m_InRange = Vector3.Distance(base.transform.position, this.m_Target.position) < this.m_Distance;
				bool flag;
				flag = this.CheckBasicFOV(this.m_ForwardDirection, this.m_Target, this.m_Angle);
				Quaternion b;
				b = (flag ? (Quaternion.LookRotation(this.m_Target.position - base.transform.position) * Quaternion.Euler(this.m_ForwardOffset)) : this.m_CurrentRotation);
				base.transform.rotation = Quaternion.Slerp(this.m_LastLookRotation, b, (flag ? this.m_Speed : (this.m_Speed * 2f)) * Time.deltaTime);
				this.m_LastLookRotation = base.transform.rotation;
			}
		}
	}

	public bool CheckBasicFOV(Transform from, Transform to, float angle)
	{
		if (this.m_InRange && !(to == null) && !(from == null) && !this.m_IgnoreTracking)
		{
			return Vector3.Angle(from.forward, to.position - from.position) < angle;
		}
		return false;
	}

	public void SetActive(bool active)
	{
		this.m_LastLookRotation = base.transform.rotation;
		this.m_IsActive = active;
	}

	public void SetIgnore(bool ignore)
	{
		this.m_IgnoreTracking = ignore;
	}

	public void SetTarget(Transform target)
	{
		this.m_Target = target;
	}

	public void SetForwardDirection(Transform forwardDirection)
	{
		this.m_ForwardDirection = forwardDirection;
	}

	public void SetOffset(Vector3 offset)
	{
		this.m_ForwardOffset = offset;
	}

	public void SetSpeed(float speed)
	{
		this.m_Speed = speed;
	}

	public void SetDistance(float distance)
	{
		this.m_Distance = distance;
	}

	public void SetAngle(float angle)
	{
		this.m_Angle = angle;
	}

	protected override void OnDisposed()
	{
		this.m_Target = null;
		base.OnDisposed();
	}
}
