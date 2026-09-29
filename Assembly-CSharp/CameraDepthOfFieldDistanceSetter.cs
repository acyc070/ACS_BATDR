using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraDepthOfFieldDistanceSetter : JMonoBehaviour
{
	private const float MAX_FOCAL_DISTANCE = 10f;

	[Header("DoF Settings")]
	[SerializeField]
	private LayerMask m_LayerMask;

	[SerializeField]
	private float m_Speed = 5f;

	[SerializeField]
	private float m_Radius = 0.015f;

	[SerializeField]
	private float m_MaxDistance = 5f;

	[SerializeField]
	private float m_CloseRadius = 1f;

	[SerializeField]
	private float m_CloseMaxDistance = 2f;

	private DepthOfField m_DOFSettings;

	private RaycastHit m_Hit;

	private float m_FocalDistance = 10f;

	private float m_ResetSpeed = 1f;

	private bool m_IsActive;

	public void Initialize(bool isActive, DepthOfField dof)
	{
		this.m_DOFSettings = dof;
		this.m_IsActive = isActive;
	}

	private void Update()
	{
		if (!this.m_IsActive || this.m_DOFSettings == null || !this.m_DOFSettings.enabled || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float num;
		num = 10f;
		if (Physics.SphereCast(base.transform.position, this.m_Radius, base.transform.forward, out this.m_Hit, this.m_MaxDistance, this.m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			num = this.m_Hit.distance;
			if (!this.m_DOFSettings.active)
			{
				this.m_DOFSettings.active = true;
			}
		}
		if (this.m_FocalDistance != num)
		{
			this.m_FocalDistance = Mathf.Lerp(t: ((num > this.m_MaxDistance && this.m_FocalDistance < num) ? this.m_ResetSpeed : this.m_Speed) * Time.deltaTime, a: this.m_FocalDistance, b: num);
			if (this.m_FocalDistance > 9.99f)
			{
				this.m_FocalDistance = 10f;
			}
			FloatParameter focusDistance;
			focusDistance = this.m_DOFSettings.focusDistance;
			focusDistance.value = this.m_FocalDistance;
			this.m_DOFSettings.focusDistance = focusDistance;
		}
		else if (!Physics.SphereCast(base.transform.position, this.m_CloseRadius, base.transform.forward, out this.m_Hit, this.m_CloseMaxDistance, this.m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			if (this.m_DOFSettings.active)
			{
				this.m_DOFSettings.active = false;
			}
		}
		else if (!this.m_DOFSettings.active)
		{
			this.m_DOFSettings.active = true;
		}
	}

	protected override void OnDisposed()
	{
		this.m_DOFSettings = null;
		base.OnDisposed();
	}
}
