using UnityEngine;

public class ObjectiveInputController : Objective
{
	private enum InputType
	{
		Crouch = 0,
		Run = 1
	}

	[SerializeField]
	private InputType m_InputType;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		if (this.m_InputType == InputType.Crouch)
		{
			GameManager.Instance.Player.PlayerMovement.UnlockCrouch();
		}
		else if (this.m_InputType == InputType.Run)
		{
			GameManager.Instance.Player.PlayerMovement.ForceUnlockRun();
		}
		base.SendOnComplete();
	}

	protected override void InternalDisable()
	{
		if (this.m_InputType == InputType.Crouch)
		{
			GameManager.Instance.Player.PlayerMovement.LockCrouch();
		}
		else if (this.m_InputType == InputType.Run)
		{
			GameManager.Instance.Player.PlayerMovement.ForceLockRun();
		}
	}
}
