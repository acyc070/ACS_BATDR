using System;
using UnityEngine;

[Serializable]
public class CharacterMovement : JDisposable
{
	[SerializeField]
	private float m_MoveSpeed = 1f;

	[SerializeField]
	private float m_RunSpeed = 2f;

	[SerializeField]
	private float m_LungeSpeed = 3f;

	[SerializeField]
	private float m_JumpSpeed = 4f;

	private readonly float m_Gravity = 0.04f;

	private float m_ActiveGravity;

	private Vector3 m_ExternalForce = Vector3.zero;

	private Vector3 m_MoveDirection = Vector3.zero;

	public float MoveSpeed => this.m_MoveSpeed;

	public float RunSpeed => this.m_RunSpeed;

	public float LungeSpeed => this.m_LungeSpeed;

	public Character Character { get; private set; }

	public Vector3 MoveDirection => this.m_MoveDirection;

	public Vector2 MovementInput { get; private set; }

	public bool PreviouslyGrounded { get; private set; }

	public bool CrouchInput { get; private set; }

	public bool JumpInput { get; private set; }

	public float CurrentSpeed { get; private set; }

	public bool CanRun { get; private set; }

	public bool IsRunning { get; private set; }

	public bool CanCrouch { get; private set; }

	public bool IsCrouched { get; private set; }

	public bool CanJump { get; private set; }

	public void Initialize(Character character)
	{
		this.Character = character;
		this.CanJump = true;
		this.CanRun = true;
		this.CanCrouch = true;
		this.PreviouslyGrounded = true;
	}

	public void UpdateInput()
	{
		this.GetJumpInput(this.Character.JumpInput);
		this.GetCrouchInput(this.Character.CrouchInput);
		this.GetMovementInput(this.Character.MoveXInput, this.Character.MoveYInput, this.Character.RunInput);
		this.CurrentSpeed *= 0.1f;
	}

	private void GetJumpInput(bool jumpInput)
	{
		if (this.JumpInput || !this.Character.Controller.isGrounded || !this.CanJump)
		{
			return;
		}
		this.JumpInput = jumpInput;
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
		}
		this.JumpInput = false;
	}

	private void GetCrouchInput(bool crouchInput)
	{
		if (!this.CrouchInput && this.Character.Controller.isGrounded && this.CanCrouch)
		{
			this.CrouchInput = crouchInput;
			if (this.CrouchInput)
			{
				this.IsCrouched = !this.IsCrouched;
			}
			this.CrouchInput = false;
		}
	}

	private void GetMovementInput(float moveX, float moveY, bool run)
	{
		float x;
		x = this.MovementInput.x;
		float num;
		num = this.MovementInput.y;
		if (this.Character.Controller.isGrounded)
		{
			x = moveX;
			num = moveY;
		}
		this.IsRunning = num > 0f && this.CanRun && run;
		if (this.IsRunning && this.IsCrouched)
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
	}

	public void Update()
	{
		this.GetMovement();
		this.GetPhysics();
		this.GetGrounding();
		this.GetCrouch();
	}

	private void GetMovement()
	{
		if (this.Character.Controller.enabled)
		{
			Vector3 vector;
			vector = this.Character.transform.forward * this.MovementInput.y + this.Character.transform.right * this.MovementInput.x;
			this.m_MoveDirection.x = vector.x;
			this.m_MoveDirection.z = vector.z;
			float y;
			y = this.MoveDirection.y;
			this.m_MoveDirection *= this.CurrentSpeed;
			this.m_MoveDirection.y = y;
			bool flag;
			flag = false;
			if (this.m_ActiveGravity <= 0f && this.Character.Controller.isGrounded)
			{
				flag = true;
			}
			if (flag)
			{
				this.m_ActiveGravity = 0f;
			}
			else
			{
				this.m_ActiveGravity -= this.m_Gravity;
				this.m_MoveDirection.y = this.m_ActiveGravity;
			}
			CollisionFlags collisionFlags;
			collisionFlags = this.Character.Controller.Move(this.MoveDirection);
			if (this.m_ActiveGravity < 0f)
			{
				collisionFlags = this.Character.Controller.Move(Vector3.up * -0.01f);
			}
			if (collisionFlags == CollisionFlags.Above && this.m_ActiveGravity > 0f)
			{
				this.m_ActiveGravity = 0f;
			}
		}
	}

	private void GetPhysics()
	{
		if (!(this.m_ExternalForce.magnitude <= 0f))
		{
			this.Character.Controller.Move(this.m_ExternalForce * Time.fixedDeltaTime);
			this.m_ExternalForce = Vector3.MoveTowards(this.m_ExternalForce, Vector3.zero, 0.95f);
		}
	}

	private void GetGrounding()
	{
		if (this.Character.Controller.isGrounded && !this.PreviouslyGrounded && this.m_ActiveGravity <= 0f)
		{
			this.m_ActiveGravity = 0f;
			this.m_ExternalForce = Vector3.zero;
		}
		this.PreviouslyGrounded = this.Character.Controller.isGrounded;
	}

	private void GetCrouch()
	{
		if (this.Character.Controller.isGrounded && this.CrouchInput)
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

	public void AddForce(Vector3 force)
	{
		this.m_ExternalForce += force;
	}

	public void SetMoveSpeed(float speed)
	{
		this.m_MoveSpeed = speed;
	}

	public void SetRunSpeed(float speed)
	{
		this.m_RunSpeed = speed;
	}

	protected override void OnDisposed()
	{
		this.Character = null;
		base.OnDisposed();
	}
}
