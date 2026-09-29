using System;
using UnityEngine;

[Serializable]
public class PlayerLook : JDisposable
{
	[SerializeField]
	private float m_Sensitivity = 3f;

	[SerializeField]
	private bool m_ClampVerticalRotation = true;

	[SerializeField]
	private float m_VerticalMinClamp = -82f;

	[SerializeField]
	private float m_VerticalMaxClamp = 82f;

	private Quaternion m_CharacterTargetRotation;

	private Quaternion m_CameraTargetRotation;

	private bool m_IsRotationInitialized;

	private bool m_IsInitialVerticalClampInitialized;

	private float m_HorizontalClamp;

	private float m_InitialVerticalMaxClamp;

	private float m_InitialVerticalMinClamp;

	private float turnSpeedBoostTimer;

	private float m_InputX;

	private float m_InputY;

	public bool clampvert => this.m_ClampVerticalRotation;

	public bool hasHorizontalLock { get; private set; }

	public void Initialize(Transform character, Transform camera)
	{
		this.ResetRotation(character, camera);
		if (!this.m_IsInitialVerticalClampInitialized)
		{
			this.m_IsInitialVerticalClampInitialized = true;
			this.m_InitialVerticalMinClamp = this.m_VerticalMinClamp;
			this.m_InitialVerticalMaxClamp = this.m_VerticalMaxClamp;
		}
	}

	public void ForceRotation(Quaternion rotation)
	{
		this.m_CharacterTargetRotation = rotation;
	}

	public void ForceCameraRotation(Quaternion rotation)
	{
		this.m_CameraTargetRotation = rotation;
	}

	public void GetInput()
	{
		float tSpeed;
		tSpeed = -0.5f;
		if (this.turnSpeedBoostTimer > 0.3f)
		{
			tSpeed = 0f;
		}
		float num;
		num = PlayerInput.LookX(tSpeed);
		if (Mathf.Abs(num) > 0.1f)
		{
			this.turnSpeedBoostTimer += Time.deltaTime;
		}
		else
		{
			this.turnSpeedBoostTimer = 0f;
		}
		float num2;
		num2 = 150f;
		float num3;
		num3 = 150f;
		if (PlayerInput.HasController)
		{
			num2 = 80f;
			num3 = 40f;
		}
		this.m_InputX = num * this.m_Sensitivity * num2 * Time.fixedDeltaTime;
		this.m_InputY = (0f - PlayerInput.LookY()) * this.m_Sensitivity * num3 * Time.fixedDeltaTime;
	}

	public void Rotation(Transform character)
	{
		this.Rotation(character, null, hasGravity: true);
	}

	public void Rotation(Transform character, bool hasGravity)
	{
		this.Rotation(character, null, hasGravity);
	}

	public void Rotation(Transform character, params Transform[] cameras)
	{
		for (int i = 0; i < cameras.Length; i = unchecked(i + 1))
		{
			if ((bool)cameras[i])
			{
				this.Rotation(character, cameras[i], hasGravity: true);
			}
		}
	}

	public void Rotation(Transform character, Transform camera)
	{
		this.Rotation(character, camera, hasGravity: true);
	}

	public void Rotation(Transform character, Transform camera, bool hasGravity)
	{
		if (!this.IsNullRotation(this.m_InputX, this.m_InputY))
		{
			if (hasGravity)
			{
				this.m_CharacterTargetRotation *= Quaternion.Euler(0f, this.m_InputX, 0f);
				if (this.hasHorizontalLock)
				{
					this.m_CharacterTargetRotation = this.ClampRotationYAxis(this.m_CharacterTargetRotation, 0f - this.m_HorizontalClamp, this.m_HorizontalClamp);
				}
				character.localRotation = this.m_CharacterTargetRotation;
				if ((bool)camera)
				{
					this.m_CameraTargetRotation *= Quaternion.Euler(this.m_InputY, 0f, 0f);
					if (this.m_ClampVerticalRotation)
					{
						this.m_CameraTargetRotation = this.ClampRotationXAxis(this.m_CameraTargetRotation, this.m_VerticalMinClamp, this.m_VerticalMaxClamp);
					}
					camera.localRotation = this.m_CameraTargetRotation;
					Vector3 localEulerAngles;
					localEulerAngles = camera.localEulerAngles;
					localEulerAngles.z = 0f;
					camera.localEulerAngles = localEulerAngles;
				}
			}
			else
			{
				this.m_CharacterTargetRotation *= Quaternion.Euler(this.m_InputY, this.m_InputX, 0f);
				character.localRotation = this.m_CharacterTargetRotation;
			}
		}
		else
		{
			character.localRotation = this.m_CharacterTargetRotation;
			if ((bool)camera)
			{
				camera.localRotation = this.m_CameraTargetRotation;
			}
		}
		this.UpdateCursorLock();
	}

	public void SmoothLook(Transform character, Transform camera, Vector3 newDirection)
	{
		Quaternion b;
		b = Quaternion.LookRotation(newDirection);
		character.localRotation = Quaternion.Lerp(character.rotation, b, 2f * Time.deltaTime);
		character.localEulerAngles = new Vector3(0f, character.localEulerAngles.y, 0f);
		camera.localRotation = Quaternion.Lerp(camera.rotation, b, 2f * Time.deltaTime);
		camera.localRotation = this.ClampRotationYAxis(this.ClampRotationXAxis(camera.localRotation, this.m_VerticalMinClamp, this.m_VerticalMaxClamp), 0f - this.m_HorizontalClamp, this.m_HorizontalClamp);
		Vector3 localEulerAngles;
		localEulerAngles = camera.localEulerAngles;
		localEulerAngles.z = 0f;
		camera.localEulerAngles = localEulerAngles;
	}

	public void ResetRotation(Transform character)
	{
		this.ResetRotation(character, null);
	}

	public void ResetRotation(Transform character, Transform camera)
	{
		this.m_CharacterTargetRotation.eulerAngles = new Vector3(0f, character.localRotation.eulerAngles.y, 0f);
		if ((bool)camera)
		{
			this.m_CameraTargetRotation.eulerAngles = new Vector3(camera.localRotation.eulerAngles.x, 0f, 0f);
		}
	}

	public void ResetVerticalClamp()
	{
		this.m_VerticalMinClamp = this.m_InitialVerticalMinClamp;
		this.m_VerticalMaxClamp = this.m_InitialVerticalMaxClamp;
	}

	public void SetVerticalClamp(float clamp)
	{
		this.m_VerticalMinClamp = 0f - clamp;
		this.m_VerticalMaxClamp = clamp;
	}

	public void SetHorizontalClamp(float clamp)
	{
		this.m_HorizontalClamp = clamp;
	}

	public void HorizontalClampSetActive(bool active)
	{
		this.hasHorizontalLock = active;
	}

	public void UpdateCursorLock()
	{
		this.InternalLockUpdate();
	}

	private void InternalLockUpdate()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private bool IsNullRotation(float horizontal, float vertical)
	{
		if (horizontal == 0f && vertical == 0f)
		{
			return true;
		}
		if (!this.m_IsRotationInitialized)
		{
			return this.m_IsRotationInitialized = true;
		}
		return false;
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

	private Quaternion ClampRotationYAxis(Quaternion _quaternion, float minimum, float maximum)
	{
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		_quaternion.y = Mathf.Tan(MathF.PI / 360f * Mathf.Clamp(114.59156f * Mathf.Atan(_quaternion.y), minimum, maximum));
		return _quaternion;
	}

	public void SetSensitivity(float sensitivity)
	{
		this.m_Sensitivity = sensitivity * 5f;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
