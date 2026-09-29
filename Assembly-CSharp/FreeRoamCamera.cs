using System;
using System.Collections;
using UnityEngine;

public class FreeRoamCamera : JMonoBehaviour
{
	private Quaternion m_VerticalRotation;

	private Quaternion m_HorizontalRotation;

	private bool m_IsActive;

	private bool m_CanActivate = true;

	private Collider m_FreeRoamCollider;

	private bool m_IsColliderEnabled = true;

	private float m_SpeedModifier = 1f;

	public override void Start()
	{
		base.gameObject.AddComponent<Rigidbody>().isKinematic = true;
		this.m_FreeRoamCollider = base.gameObject.AddComponent<SphereCollider>();
		this.m_FreeRoamCollider.isTrigger = true;
		base.gameObject.tag = "Player";
		this.m_FreeRoamCollider.enabled = false;
	}

	private void Update()
	{
		if (this.m_CanActivate && !(GameManager.Instance.Player == null) && !(GameManager.Instance.GameCamera == null) && GameManager.Instance.GameState != GameState.UI && !base.IsDisposed)
		{
			_ = GameManager.Instance.IsPaused;
		}
	}

	private IEnumerator ActivatePlayer()
	{
		yield return new WaitForSeconds(0.01f);
		GameManager.Instance.ShowCrosshair();
		this.m_CanActivate = true;
	}

	private Quaternion ClampRotationXAxis(Quaternion _quaternion, float minimum, float maximum)
	{
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		_quaternion.x = Mathf.Tan(MathF.PI / 360f * Mathf.Clamp(114.59156f * Mathf.Atan(_quaternion.x), minimum, maximum));
		return _quaternion;
	}
}
