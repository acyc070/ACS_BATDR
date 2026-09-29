public static class CameraEffects
{
	public static void ShakeRotation(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool vibrate = true)
	{
		GameManager.Instance.GameCamera.ShakeCamera(duration, strength, vibrato, randomness, fadeOut, vibrate);
	}

	public static void Damage(float endDuration = 3f)
	{
		GameManager.Instance.GameCamera.Damage(endDuration);
	}

	public static void Takedown(float endDuration = 3f)
	{
		GameManager.Instance.GameCamera.Takedown(endDuration);
	}

	public static void GainPower(float endDuration = 3f)
	{
		GameManager.Instance.GameCamera.GainPower(endDuration);
	}

	public static void InkDemon(float endDuration = 3f)
	{
		GameManager.Instance.GameCamera.InkDemon(endDuration);
	}

	public static void InkDemonEffectOn()
	{
		GameManager.Instance.GameCamera.InkDemonOn();
	}

	public static void InkDemonEffectOff()
	{
		GameManager.Instance.GameCamera.InkDemonOff();
	}

	public static void VisionOn()
	{
		GameManager.Instance.GameCamera.VisionOn();
	}

	public static void VisionOff()
	{
		GameManager.Instance.GameCamera.VisionOff();
	}

	public static void VisionTransition(bool active)
	{
		GameManager.Instance.GameCamera.VisionTransition(active);
	}
}
