using System;
using UnityEngine;

[Serializable]
public class PlayerMovement : JDisposable
{
	[Header("Movement Options")]
	[SerializeField]
	private float m_MoveSpeed = 2f;

	[SerializeField]
	private bool m_CanRun = true;

	[SerializeField]
	private float m_RunSpeed = 4f;

	[SerializeField]
	private bool m_CanJump = true;

	[SerializeField]
	private float m_JumpSpeed = 4f;

	[SerializeField]
	private bool m_CanCrouch = true;

	private readonly float m_Gravity = 0.025f;

	private float m_ActiveGravity;

	private Vector3 m_ExternalForce = Vector3.zero;

	private float m_DefaultMoveSpeed = 2f;

	private CharacterController m_CharacterController;

	private Vector3 m_MoveDirection = Vector3.zero;

	public bool IsSlowed;

	private Vector3 m_InitialAirPosition;

	public Vector3 MoveDirection => this.m_MoveDirection;

	public Vector2 MovementInput { get; private set; }

	public Vector3 PreviousPosition { get; private set; }

	public Vector3 CurrentPosition { get; private set; }

	public bool PreviouslyGrounded { get; private set; }

	public bool CrouchInput { get; private set; }

	public bool JumpInput { get; private set; }

	public float CurrentSpeed { get; private set; }

	public bool CanRun { get; private set; }

	public bool IsRunning { get; private set; }

	public bool IsRunLocked { get; private set; }

	public bool CanCrouch { get; private set; }

	public bool IsCrouched { get; private set; }

	public bool CanJump { get; private set; }

	public Vector3 InitialAirPosition
	{
		get
		{
			return this.m_InitialAirPosition;
		}
		set
		{
			this.m_InitialAirPosition = value;
		}
	}

	public void LockRun()
	{
		this.IsRunLocked = true;
	}

	public void UnlockRun()
	{
		this.IsRunLocked = false;
	}

	public void StopRun()
	{
		this.IsRunning = false;
	}

	public void ForceLockRun()
	{
		this.CanRun = false;
	}

	public void ForceUnlockRun()
	{
		this.CanRun = true;
	}

	public void LockCrouch()
	{
		this.CanCrouch = false;
	}

	public void UnlockCrouch()
	{
		this.CanCrouch = true;
	}

	public void LockJump()
	{
		this.CanJump = false;
	}

	public void UnlockJump()
	{
		this.CanJump = true;
	}

	public void Initialize(CharacterController characterController)
	{
		this.m_CharacterController = characterController;
		this.CanJump = this.m_CanJump;
		this.CanRun = this.m_CanRun;
		this.CanCrouch = this.m_CanCrouch;
		this.PreviouslyGrounded = true;
		this.m_DefaultMoveSpeed = this.m_MoveSpeed;
		this.m_MoveDirection.y = -0.27f;
	}

	public void SetMoveSpeed(float speed, bool _isSlowed = false)
	{
		this.m_MoveSpeed = speed;
		this.IsSlowed = _isSlowed;
	}

	public void ResetMoveSpeed()
	{
		this.m_MoveSpeed = this.m_DefaultMoveSpeed;
		this.IsSlowed = false;
	}

	public void UpdateMovementInput()
	{
		this.GetJumpInput();
		this.GetCrouchInput();
		this.GetMovementInput();
	}

	private void GetJumpInput()
	{
		if (this.JumpInput || !this.m_CharacterController.isGrounded || !this.CanJump)
		{
			return;
		}
		if (!Physics.SphereCast(this.m_CharacterController.transform.position, this.m_CharacterController.radius, Vector3.up, out var _, 6.5f - this.m_CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
		{
			this.JumpInput = PlayerInput.Jump();
			if (this.JumpInput)
			{
				if (this.IsRunning && this.MovementInput.y > 0f)
				{
					this.m_ActiveGravity += this.m_JumpSpeed * 0.11f;
				}
				else
				{
					this.m_ActiveGravity += this.m_JumpSpeed * 0.1f;
				}
				this.IsCrouched = false;
			}
		}
		this.JumpInput = false;
	}

	private void GetCrouchInput()
	{
		if (!this.CrouchInput && this.m_CharacterController.isGrounded && this.CanCrouch)
		{
			this.CrouchInput = PlayerInput.Crouch();
			if (this.CrouchInput)
			{
				this.IsCrouched = !this.IsCrouched;
			}
			this.CrouchInput = false;
		}
	}

	private void GetMovementInput()
	{
		float x;
		x = this.MovementInput.x;
		float num;
		num = this.MovementInput.y;
		if (this.m_CharacterController.isGrounded)
		{
			x = PlayerInput.MoveX();
			num = PlayerInput.MoveY();
		}
		else
		{
			if (this.MovementInput.x == 0f)
			{
				x = PlayerInput.MoveXRaw() / 2f;
			}
			if (this.MovementInput.y == 0f)
			{
				num = PlayerInput.MoveYRaw() / 2f;
			}
		}
		this.IsRunning = num > 0f && this.CanRun && !this.IsRunLocked && PlayerInput.Run();
		if (this.IsRunning && this.IsCrouched && !Physics.SphereCast(this.m_CharacterController.transform.position, this.m_CharacterController.radius, Vector3.up, out var _, 6.5f - this.m_CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
		{
			this.IsCrouched = false;
		}
		if (this.IsCrouched)
		{
			this.IsRunning = false;
		}
		this.CurrentSpeed = (this.IsRunning ? this.m_RunSpeed : this.m_MoveSpeed);
		if (this.IsCrouched)
		{
			this.CurrentSpeed = this.m_MoveSpeed / 2f;
		}
		this.MovementInput = new Vector2(x, num);
		if (this.MovementInput.magnitude > 1f || this.MovementInput.magnitude < -1f)
		{
			this.MovementInput.Normalize();
		}
		this.CurrentSpeed *= 0.1f;
	}

	public void UpdateMovement(Transform transform)
	{
		this.GetMovement(transform);
		this.GetPhysics();
		this.GetGrounding();
		this.GetCrouch();
	}

	private void GetMovement(Transform transform)
	{
		if (this.m_CharacterController.enabled)
		{
			Vector3 vector;
			vector = transform.forward * this.MovementInput.y + transform.right * this.MovementInput.x;
			this.m_MoveDirection.x = vector.x;
			this.m_MoveDirection.z = vector.z;
			float y;
			y = this.MoveDirection.y;
			if (this.m_MoveDirection.z > 1f)
			{
				this.m_MoveDirection.z = 1f;
			}
			else if (this.m_MoveDirection.z < -1f)
			{
				this.m_MoveDirection.z = -1f;
			}
			if (this.m_MoveDirection.x > 1f)
			{
				this.m_MoveDirection.x = 1f;
			}
			else if (this.m_MoveDirection.x < -1f)
			{
				this.m_MoveDirection.x = -1f;
			}
			this.m_MoveDirection = this.m_MoveDirection.normalized * this.CurrentSpeed;
			this.m_MoveDirection.y = y;
			bool flag;
			flag = false;
			if (this.m_ActiveGravity <= 0f && this.m_CharacterController.isGrounded)
			{
				flag = true;
			}
			if (flag)
			{
				this.m_ActiveGravity = 0f;
				this.m_MoveDirection.y = -0.27f;
			}
			else
			{
				this.m_ActiveGravity -= this.m_Gravity;
				this.m_MoveDirection.y = this.m_ActiveGravity;
			}
			this.PreviousPosition = this.m_CharacterController.transform.position;
			CollisionFlags collisionFlags;
			collisionFlags = this.m_CharacterController.Move(this.MoveDirection);
			if (this.m_ActiveGravity < 0f)
			{
				collisionFlags = this.m_CharacterController.Move(Vector3.up * -0.01f);
			}
			if (collisionFlags == CollisionFlags.Above && this.m_ActiveGravity > 0f)
			{
				this.m_ActiveGravity = 0f;
			}
			this.CurrentPosition = this.m_CharacterController.transform.position;
		}
	}

	private void GetPhysics()
	{
		if (!(this.m_ExternalForce.magnitude <= 0f))
		{
			this.m_CharacterController.Move(this.m_ExternalForce * Time.fixedDeltaTime);
			this.m_ExternalForce = Vector3.MoveTowards(this.m_ExternalForce, Vector3.zero, 0.95f);
		}
	}

	private void GetGrounding()
	{
		if (this.m_CharacterController.isGrounded && !this.PreviouslyGrounded && this.m_ActiveGravity <= 0f)
		{
			this.m_ActiveGravity = 0f;
		}
		if (this.PreviouslyGrounded)
		{
			this.m_InitialAirPosition = this.m_CharacterController.transform.position;
		}
		this.PreviouslyGrounded = this.m_CharacterController.isGrounded;
	}

	public void SetPreviouslyGrounded(bool active)
	{
		this.PreviouslyGrounded = active;
	}

	private void GetCrouch()
	{
		if (this.m_CharacterController.isGrounded && this.CrouchInput)
		{
			this.IsCrouched = !this.IsCrouched;
		}
	}

	public void CancelMovement()
	{
		this.MovementInput = Vector3.zero;
		this.CurrentSpeed = 0f;
		this.m_ActiveGravity = 0f;
		this.m_ExternalForce = Vector3.zero;
		this.CrouchInput = false;
		this.JumpInput = false;
	}

	public void ForceStand()
	{
		this.IsCrouched = false;
		this.CrouchInput = false;
	}

	public void ForceCrouch()
	{
		this.UnlockCrouch();
		this.IsCrouched = true;
		this.CrouchInput = false;
	}

	public void AddForce(Vector3 force)
	{
		this.m_ExternalForce += force;
	}

	public void SetCrouchInput(bool isCrouched)
	{
		this.IsCrouched = isCrouched;
	}

	protected override void OnDisposed()
	{
		this.m_CharacterController = null;
		base.OnDisposed();
	}
}
