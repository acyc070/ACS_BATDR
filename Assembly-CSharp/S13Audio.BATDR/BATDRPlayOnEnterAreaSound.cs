using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayOnEnterAreaSound : MonoBehaviour
{
	public S13AreaHandlerReference targetAreaSound;

	public List<S13AudioHandler> audioHandlers;

	private void Awake()
	{
		if (this.targetAreaSound != null)
		{
			this.targetAreaSound.onAreaEnter += this.Play;
			this.targetAreaSound.onAreaExit += this.Stop;
		}
	}

	private void OnDestroy()
	{
		if (this.targetAreaSound != null)
		{
			this.targetAreaSound.onAreaEnter -= this.Play;
			this.targetAreaSound.onAreaExit -= this.Stop;
		}
	}

	public void Play()
	{
		foreach (S13AudioHandler audioHandler in this.audioHandlers)
		{
			audioHandler.Play();
		}
	}

	public void Stop()
	{
		foreach (S13AudioHandler audioHandler in this.audioHandlers)
		{
			audioHandler.Stop();
		}
	}
}
