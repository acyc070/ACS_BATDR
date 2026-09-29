using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
	public Vector2 MovementInput { get; private set; }

	public bool PreviouslyGrounded { get; private set; }

	public bool CrouchInput { get; private set; }

	public bool JumpInput { get; private set; }

	private void Update()
	{
		this.JumpInput = PlayerInput.Jump();
		this.CrouchInput = PlayerInput.Crouch();
	}
}
