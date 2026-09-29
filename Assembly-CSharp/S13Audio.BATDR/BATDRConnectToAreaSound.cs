using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRConnectToAreaSound : MonoBehaviour
{
	[SerializeField]
	private S13AreaHandlerReference areaSound;

	private bool _targetWithinArea;

	public S13LocalAction onEnterArea;

	public S13LocalAction onExitArea;

	private void OnEnable()
	{
		if (this.areaSound != null)
		{
			this.areaSound.onAreaEnter += this.OnAreaEnter;
			this.areaSound.onAreaExit += this.OnAreaExit;
		}
	}

	private void OnDisable()
	{
		if (this.areaSound != null)
		{
			this.areaSound.onAreaEnter -= this.OnAreaEnter;
			this.areaSound.onAreaExit -= this.OnAreaExit;
		}
	}

	private void OnAreaEnter()
	{
		if (this.onEnterArea.IsExecutable)
		{
			this.onEnterArea.Execute();
		}
	}

	private void OnAreaExit()
	{
		if (this.onExitArea.IsExecutable)
		{
			this.onExitArea.Execute();
		}
	}
}
