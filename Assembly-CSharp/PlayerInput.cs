using InControl;
using UnityEngine;

public class PlayerInput
{
	public static bool IsInverted;

	public static float Sensitivity;

	public static bool HasController { get; private set; }

	public static bool CheckController()
	{
		if (!PlayerInput.HasController && InputManager.ActiveDevice.IsActive)
		{
			PlayerInput.HasController = true;
		}
		else if (!InputManager.ActiveDevice.IsActive && (PlayerInput.LookXRaw() != 0f || PlayerInput.LookYRaw() != 0f))
		{
			PlayerInput.HasController = false;
		}
		return PlayerInput.HasController;
	}

	public static bool Any()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action1) || GamepadInput.GetButtonDown(InputControlType.Action2) || GamepadInput.GetButtonDown(InputControlType.Action3) || GamepadInput.GetButtonDown(InputControlType.Action4), Input.anyKeyDown);
	}

	public static float MoveX()
	{
		return PlayerInput.GetInput(GamepadInput.GetAxis(InputControlType.LeftStickX), Input.GetAxis("Horizontal"));
	}

	public static float MoveY()
	{
		return PlayerInput.GetInput(GamepadInput.GetAxis(InputControlType.LeftStickY), Input.GetAxis("Vertical"));
	}

	public static float MoveXRaw()
	{
		return PlayerInput.GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickX), Input.GetAxisRaw("Horizontal"));
	}

	public static float MoveYRaw()
	{
		return PlayerInput.GetInput(GamepadInput.GetAxisRaw(InputControlType.LeftStickY), Input.GetAxisRaw("Vertical"));
	}

	public static float LookX(float TSpeed = 1f)
	{
		float num;
		num = 3.5f;
		unchecked
		{
			int num2;
			num2 = (int)(PlayerInput.Sensitivity * 10f);
			if (num2 <= 0)
			{
				num2 = 0;
			}
			else if (num2 >= 10)
			{
				num2 = 10;
			}
			float num3;
			num3 = 0f;
			num += ((num2 > 5) ? ((float)(num2 - 5) * 0.5f) : (0f - (float)(5 - num2) * 0.5f));
			return PlayerInput.GetInput(GamepadInput.GetAxis(InputControlType.RightStickX) * (num * 1.5f + TSpeed), Input.GetAxis("Mouse X") * (num / 4f));
		}
	}

	public static float LookY()
	{
		float num;
		num = 3f;
		unchecked
		{
			int num2;
			num2 = (int)(PlayerInput.Sensitivity * 10f);
			if (num2 <= 0)
			{
				num2 = 0;
			}
			else if (num2 >= 10)
			{
				num2 = 10;
			}
			float num3;
			num3 = 0f;
			num += ((num2 > 5) ? ((float)(num2 - 5) * 0.4f) : (0f - (float)(5 - num2) * 0.4f));
			int num4;
			num4 = ((!PlayerInput.IsInverted) ? 1 : (-1));
			return PlayerInput.GetInput(GamepadInput.GetAxis(InputControlType.RightStickY) * (num * 1.5f), Input.GetAxis("Mouse Y") * (num / 4f)) * (float)num4;
		}
	}

	public static float LookXRaw()
	{
		return PlayerInput.GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickX), Input.GetAxisRaw("Mouse X"));
	}

	public static float LookYRaw()
	{
		int num;
		num = ((!PlayerInput.IsInverted) ? 1 : (-1));
		return PlayerInput.GetInput(GamepadInput.GetAxisRaw(InputControlType.RightStickY), Input.GetAxisRaw("Mouse Y")) * (float)num;
	}

	public static bool Cancel()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetKeyDown(KeyCode.F));
	}

	public static bool Attack()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.RightTrigger), Input.GetMouseButtonDown(0));
	}

	public static bool AttackHold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.RightTrigger), Input.GetMouseButton(0));
	}

	public static bool AttackUp()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.RightTrigger), Input.GetMouseButtonUp(0));
	}

	public static bool AttackSecondary()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.LeftTrigger), Input.GetMouseButtonDown(1));
	}

	public static bool AttackSecondaryHold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.LeftTrigger), Input.GetMouseButton(1));
	}

	public static bool AttackSecondaryRelease()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.LeftTrigger), Input.GetMouseButtonUp(1));
	}

	public static bool AttackCharge()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.RightBumper), Input.GetKeyDown(KeyCode.Tab));
	}

	public static bool Run()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.LeftStickButton) || GamepadInput.GetButton(InputControlType.LeftBumper), Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
	}

	public static bool Jump()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetKeyDown(KeyCode.Space));
	}

	public static bool JumpButtonUp()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), Input.GetKeyUp(KeyCode.Space));
	}

	public static bool JumpHold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.Action1), Input.GetKey(KeyCode.Space));
	}

	public static bool Crouch()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.RightStickButton), Input.GetKeyDown(KeyCode.C));
	}

	public static bool CrouchButtonUp()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.RightStickButton), Input.GetKeyUp(KeyCode.C));
	}

	public static bool Pause()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Start) || GamepadInput.GetButtonDown(InputControlType.Options) || GamepadInput.GetButtonDown(InputControlType.Menu), Input.GetKeyDown(KeyCode.Escape));
	}

	public static bool SpecialActionM()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action4), Input.GetKeyDown(KeyCode.M));
	}

	public static bool SpecialAction()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action4), Input.GetKeyDown(KeyCode.Y));
	}

	public static bool SpecialActionReleased()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action4), Input.GetKeyUp(KeyCode.Y));
	}

	public static bool SpecialActionHold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.Action4), Input.GetKey(KeyCode.Y));
	}

	public static bool InteractOnPressed()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetKeyDown(KeyCode.E));
	}

	public static bool InteractOnReleased()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetKeyUp(KeyCode.E));
	}

	public static bool InteractOnHold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.Action3), Input.GetKey(KeyCode.E));
	}

	public static float ControllerY()
	{
		float axisRaw;
		axisRaw = GamepadInput.GetAxisRaw(InputControlType.LeftStickY);
		if (axisRaw == 0f)
		{
			axisRaw = GamepadInput.GetAxisRaw(InputControlType.DPadY);
		}
		return PlayerInput.GetInput(axisRaw, 0f);
	}

	public static float ControllerX()
	{
		float axisRaw;
		axisRaw = GamepadInput.GetAxisRaw(InputControlType.LeftStickX);
		if (axisRaw == 0f)
		{
			axisRaw = GamepadInput.GetAxisRaw(InputControlType.DPadX);
		}
		return PlayerInput.GetInput(axisRaw, 0f);
	}

	public static bool ControllerAction1()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction1Hold()
	{
		return PlayerInput.GetInput(GamepadInput.GetButton(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction1Up()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), inputKeyboard: false);
	}

	public static bool ControllerAction2()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), inputKeyboard: false);
	}

	public static bool ControllerAction3()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), inputKeyboard: false);
	}

	public static bool ControllerStart()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Start) || GamepadInput.GetButtonDown(InputControlType.Options) || GamepadInput.GetButtonDown(InputControlType.Menu), inputKeyboard: false);
	}

	public static bool ControllerLeftBumper()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.LeftBumper), inputKeyboard: false);
	}

	public static bool ControllerRightBumper()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.RightBumper), inputKeyboard: false);
	}

	public static bool ControllerLeftTrigger()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.LeftTrigger), inputKeyboard: false);
	}

	public static bool ControllerRightTrigger()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.RightTrigger), inputKeyboard: false);
	}

	public static bool VirtualMouseLeftOnPressed()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action1), Input.GetMouseButtonDown(0));
	}

	public static bool VirtualMouseLeftOnReleased()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action1), Input.GetMouseButtonUp(0));
	}

	public static bool VirtualMouseRightOnPressed()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action2), Input.GetMouseButtonDown(1));
	}

	public static bool VirtualMouseRightOnReleased()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action2), Input.GetMouseButtonUp(1));
	}

	public static bool VirtualMouseMiddleOnPressed()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonDown(InputControlType.Action3), Input.GetMouseButtonDown(2));
	}

	public static bool VirtualMouseMiddleOnReleased()
	{
		return PlayerInput.GetInput(GamepadInput.GetButtonUp(InputControlType.Action3), Input.GetMouseButtonUp(2));
	}

	private static T GetInput<T>(T inputGamepad, T inputKeyboard)
	{
		T result;
		result = default;
		if (!inputGamepad.Equals(default(T)))
		{
			PlayerInput.HasController = true;
			return inputGamepad;
		}
		if (!inputKeyboard.Equals(default(T)))
		{
			PlayerInput.HasController = false;
			return inputKeyboard;
		}
		return result;
	}

	static PlayerInput()
	{
		PlayerInput.Sensitivity = 1f;
	}
}
