using UnityEngine;

namespace S13Audio.BATDR;

public class BATRDRInkWidowSwarmEntry : MonoBehaviour
{
	private void OnEnable()
	{
		BATDRInkWidowSwarmMonitor.RegisterInkWidow(this);
	}

	private void OnDisable()
	{
		BATDRInkWidowSwarmMonitor.DeRegisterInkWidow(this);
	}
}
