using UnityEngine;

public class TimeScaler : JMonoBehaviour
{
	public void SetTimeScale(float value = 1f)
	{
		Time.timeScale = value;
	}
}
