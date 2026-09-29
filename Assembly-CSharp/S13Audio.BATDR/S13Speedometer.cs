using System;
using UnityEngine;

namespace S13Audio.BATDR;

[DefaultExecutionOrder(1)]
public class S13Speedometer : MonoBehaviour
{
	private enum SpeedCalcMethod
	{
		Static = 0,
		PlayerMovement = 1
	}

	private enum SpeedScaleMethod
	{
		Ignore = 0,
		Static = 1,
		ScaleMagnitude = 2
	}

	[S13LockableField]
	[SerializeField]
	private string accessorID = string.Empty;

	[SerializeField]
	private SpeedCalcMethod speedCalcMethod = SpeedCalcMethod.PlayerMovement;

	[SerializeField]
	private SpeedScaleMethod speedScaleMethod = SpeedScaleMethod.Static;

	[SerializeField]
	private Player player;

	[Header("Falling Settings")]
	[SerializeField]
	private bool enableFallspeedCalculation = true;

	[SerializeField]
	[Tooltip("If Y movement is less than this value, begin falling")]
	private float beginFallingYMovement = -0.4f;

	[SerializeField]
	[Tooltip("The Y movement that is considered to be max falling speed")]
	private float maxFallingYMovement = -0.7f;

	[SerializeField]
	[S13CurveRange(0f, 0f, 1f, 1f)]
	private AnimationCurve fallSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[ReadOnly]
	[SerializeField]
	private float curSpeed;

	private float lastSpeed;

	private void Start()
	{
		if (!this.player)
		{
			S13Debug.LogWarning("Player not assigned to S13SpeedParameter. Disabling");
			base.enabled = false;
		}
	}

	private void Update()
	{
		this.curSpeed = 0f;
		float num;
		num = 1f;
		switch (this.speedCalcMethod)
		{
		case SpeedCalcMethod.Static:
			if (this.player.PlayerMovement.IsCrouched)
			{
				this.curSpeed = 3.75f;
			}
			else if (this.player.PlayerMovement.IsRunning)
			{
				this.curSpeed = 14.5f;
			}
			else
			{
				this.curSpeed = 7.75f;
			}
			break;
		case SpeedCalcMethod.PlayerMovement:
			this.curSpeed = this.player.PlayerMovement.CurrentSpeed * 3.33f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		switch (this.speedScaleMethod)
		{
		case SpeedScaleMethod.Static:
			if (this.player.PlayerMovement.MovementInput.magnitude <= 0f)
			{
				num = 0f;
			}
			break;
		case SpeedScaleMethod.ScaleMagnitude:
			num = Mathf.Clamp01(this.player.PlayerMovement.MovementInput.magnitude);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case SpeedScaleMethod.Ignore:
			break;
		}
		this.curSpeed *= num;
		float num2;
		num2 = 0f;
		if (this.enableFallspeedCalculation)
		{
			if (this.player.PlayerMovement.MoveDirection.y < this.beginFallingYMovement)
			{
				float time;
				time = this.Normalize01(this.player.PlayerMovement.MoveDirection.y, this.beginFallingYMovement, this.maxFallingYMovement);
				num2 = this.fallSpeedCurve.Evaluate(time);
			}
			this.curSpeed += num2;
		}
		this.curSpeed = Mathf.Clamp01(this.curSpeed);
		if (!Mathf.Approximately(this.curSpeed, this.lastSpeed))
		{
			this.lastSpeed = this.curSpeed;
			S13Manager.SetAccessor(this.accessorID, this.curSpeed);
		}
	}

	private float Normalize01(float value, float minInput, float maxInput)
	{
		return Mathf.Abs(value - minInput) / Mathf.Abs(maxInput - minInput);
	}
}
