using System;
using UnityEngine;

[Serializable]
public class CameraMovements : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private float m_SwaySpeed = 0.6f;

	[SerializeField]
	private float m_BaseSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingSwayAmount = 1.5f;

	[SerializeField]
	private float m_TrackingBias;

	[SerializeField]
	private float m_FollowSpeed = 1f;

	private Transform m_Target;

	private Quaternion m_OriginalRotation;

	private Vector3 m_FollowVelocity;

	private Vector3 m_FollowAngles;

	private Vector2 m_RotationRange;

	public bool IsActive => this.m_IsActive;

	public override void Awake()
	{
		base.Awake();
		this.m_OriginalRotation = base.transform.localRotation;
		this.m_Target = new GameObject("Forward Camera Target").transform;
		this.m_Target.SetParent(base.transform);
		this.m_Target.localPosition = Vector3.forward;
		this.m_Target.localEulerAngles = Vector3.zero;
		this.m_IsActive = GameManager.Instance.PlayerSettings.ViewSwaying;
	}

	private void Update()
	{
		if (this.m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.Sway();
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
		if (!this.m_IsActive)
		{
			base.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void Sway()
	{
		base.transform.localRotation = this.m_OriginalRotation;
		Vector3 vector;
		vector = base.transform.parent.InverseTransformPoint(this.m_Target.position);
		float num;
		num = Mathf.Clamp(Mathf.Atan2(vector.x, vector.z) * 57.29578f, -10.5f, 10.5f);
		base.transform.localRotation = this.m_OriginalRotation * Quaternion.Euler(0f, num, 0f);
		vector = base.transform.parent.InverseTransformPoint(this.m_Target.position);
		float target;
		target = Mathf.Clamp(Mathf.Atan2(vector.y, vector.z) * 57.29578f, -10.5f, 10.5f);
		this.m_FollowAngles = Vector3.SmoothDamp(target: new Vector3(this.m_FollowAngles.x + Mathf.DeltaAngle(this.m_FollowAngles.x, target), this.m_FollowAngles.y + Mathf.DeltaAngle(this.m_FollowAngles.y, num)), current: this.m_FollowAngles, currentVelocity: ref this.m_FollowVelocity, smoothTime: this.m_FollowSpeed);
		base.transform.localRotation = this.m_OriginalRotation * Quaternion.Euler(0f - this.m_FollowAngles.x, this.m_FollowAngles.y, 0f);
		float num2;
		num2 = Mathf.PerlinNoise(0f, Time.time * this.m_SwaySpeed) - 0.5f;
		float num3;
		num3 = Mathf.PerlinNoise(0f, Time.time * this.m_SwaySpeed + 100f) - 0.5f;
		num2 *= this.m_BaseSwayAmount;
		float num4;
		num4 = num3 * this.m_BaseSwayAmount;
		float num5;
		num5 = Mathf.PerlinNoise(0f, Time.time * this.m_SwaySpeed) - 0.5f + this.m_TrackingBias;
		float num6;
		num6 = Mathf.PerlinNoise(0f, Time.time * this.m_SwaySpeed + 100f) - 0.5f + this.m_TrackingBias;
		num5 *= (0f - this.m_TrackingSwayAmount) * this.m_FollowVelocity.x;
		num6 *= this.m_TrackingSwayAmount * this.m_FollowVelocity.y;
		float xAngle;
		xAngle = num2 + num5;
		float num7;
		num7 = num4 + num6;
		base.transform.Rotate(xAngle, num7, (0f - num7) * 0.5f);
	}

	protected override void OnDisposed()
	{
		this.m_Target = null;
		base.OnDisposed();
	}
}
