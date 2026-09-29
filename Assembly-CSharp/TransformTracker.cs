using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TransformTracker : JComponent
{
	[Serializable]
	public class ClampRotations
	{
		public bool IsActive;

		public float LowClamp = 230f;

		public float HighClamp = 290f;
	}

	[SerializeField]
	private bool m_IsActive = true;

	[Header("Transform Options")]
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private Vector3 m_ForwardOffset = new Vector3(0f, 270f, 270f);

	[SerializeField]
	private float m_Speed = 15f;

	[Header("Clamp Options")]
	[SerializeField]
	private ClampRotations m_ClampRotationX;

	[SerializeField]
	private ClampRotations m_ClampRotationY;

	[SerializeField]
	private ClampRotations m_ClampRotationZ;

	private Quaternion m_LastLookRotation;

	private Vector3 m_ClampRotation;

	public override void Awake()
	{
		this.m_LastLookRotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		if (this.m_IsActive && !(this.m_Target == null))
		{
			Quaternion quaternion;
			quaternion = base.transform.rotation;
			if (this.m_IsActive)
			{
				quaternion = Quaternion.LookRotation(this.m_Target.forward) * Quaternion.Euler(this.m_ForwardOffset);
			}
			this.m_ClampRotation = quaternion.eulerAngles;
			this.SetClamp(ref this.m_ClampRotation.x, ref this.m_ClampRotationX);
			this.SetClamp(ref this.m_ClampRotation.y, ref this.m_ClampRotationY);
			this.SetClamp(ref this.m_ClampRotation.z, ref this.m_ClampRotationZ);
			if (this.m_IsActive)
			{
				base.transform.rotation = Quaternion.Slerp(this.m_LastLookRotation, Quaternion.Euler(this.m_ClampRotation), this.m_Speed * Time.deltaTime);
			}
			this.m_LastLookRotation = base.transform.rotation;
		}
	}

	private void SetClamp(ref float rotation, ref ClampRotations axis)
	{
		if (axis.IsActive)
		{
			if (rotation < axis.LowClamp)
			{
				rotation = axis.LowClamp;
			}
			else if (rotation > axis.HighClamp)
			{
				rotation = axis.HighClamp;
			}
		}
	}

	public void SetActive(bool active)
	{
		this.m_LastLookRotation = base.transform.rotation;
		this.m_IsActive = active;
	}

	public void SetTarget(Transform target)
	{
		this.m_Target = target;
	}
}
