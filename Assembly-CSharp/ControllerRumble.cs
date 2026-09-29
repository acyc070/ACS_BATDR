using DG.Tweening;
using InControl;
using UnityEngine;

public class ControllerRumble
{
	public enum RUMBLE_PRESETS
	{
		UI_Blip = 0,
		Strong_Jolt = 1,
		Soft_Drag = 2,
		Long_Pop = 3,
		Note_Pickup = 4,
		Desk_Drawers = 5,
		Food_Pickup = 6,
		Inv_Pickup = 7,
		Cart_Drag = 8,
		NONE = 9
	}

	private const float PS_RUMBLE_INTENSITY_MODIFIER = 1.15f;

	private const float XBOX_RUMBLE_INTENSITY_MODIFIER = 1f;

	private const float SWITCH_RUMBLE_INTENSITY_MODIFIER = 1f;

	public static readonly Vector2[] rumblePresets;

	private Tweener m_ControllerVibrationTweener;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetVibration(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool isIndependentUpdate = false)
	{
		float num;
		num = strength;
		switch (Application.platform)
		{
		case RuntimePlatform.PS4:
		case RuntimePlatform.PS5:
			num *= 1.15f;
			break;
		case RuntimePlatform.GameCoreXboxSeries:
		case RuntimePlatform.GameCoreXboxOne:
			num *= 1f;
			break;
		case RuntimePlatform.Switch:
			num *= 1f;
			break;
		}
		num = (num + 0.5f) * GameManager.Instance.PlayerSettings.Rumble;
		if (duration == 0f || strength == 0f)
		{
			return;
		}
		this.m_ControllerVibrationTweener.Kill();
		if (!GameManager.Instance.HasController)
		{
			return;
		}
		float vibration;
		vibration = num;
		this.m_ControllerVibrationTweener = DOTween.To(() => vibration, (float x) =>
		{
			vibration = x;
		}, 0f, duration).SetUpdate(UpdateType.Normal, isIndependentUpdate).OnUpdate(() =>
		{
			if (GameManager.Instance.HasController)
			{
				InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
			}
		})
			.OnComplete(() =>
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
	}

	public void SetVibration(Vector2 DurationAndStrength, bool isIndependentUpdate = false)
	{
		DurationAndStrength.y *= GameManager.Instance.PlayerSettings.Rumble;
		if (DurationAndStrength.y == 0f)
		{
			return;
		}
		this.m_ControllerVibrationTweener.Kill();
		if (!GameManager.Instance.HasController)
		{
			return;
		}
		float vibration;
		vibration = DurationAndStrength.y + 0.5f;
		this.m_ControllerVibrationTweener = DOTween.To(() => vibration, (float x) =>
		{
			vibration = x;
		}, 0f, DurationAndStrength.x).SetUpdate(UpdateType.Normal, isIndependentUpdate).OnUpdate(() =>
		{
			if (GameManager.Instance.HasController)
			{
				InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
			}
		})
			.OnComplete(() =>
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
	}

	public void RumbleEvent(AnimationEvent animationEvent)
	{
		float strength;
		strength = float.Parse(animationEvent.stringParameter) * GameManager.Instance.PlayerSettings.Rumble;
		GameManager.Instance.TriggerRumble(animationEvent.floatParameter, strength);
	}

	static ControllerRumble()
	{
		ControllerRumble.rumblePresets = new Vector2[9]
		{
			new Vector2(0.1f, 1f),
			new Vector2(0.3f, 3.5f),
			new Vector2(1f, 0.7f),
			new Vector2(0.5f, 2f),
			new Vector2(0.1f, 0.5f),
			new Vector2(0.6f, 0.6f),
			new Vector2(0.35f, 0.5f),
			new Vector2(0.45f, 0.3f),
			new Vector2(1.2f, 0.5f)
		};
	}
}
