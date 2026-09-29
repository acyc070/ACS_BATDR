using System;

namespace S13Audio.BATDR;

public static class BATDRPlayerHealthAudioController
{
	public const int maxHealth = 10;

	public static Action<float> OnHealthChanged;

	public static void SetPlayerHealth(float value)
	{
		float obj;
		obj = value / 10f;
		BATDRPlayerHealthAudioController.OnHealthChanged?.Invoke(obj);
	}
}
