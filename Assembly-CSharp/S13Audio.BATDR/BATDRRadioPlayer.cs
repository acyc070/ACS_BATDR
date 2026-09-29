using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRRadioPlayer : MonoBehaviour
{
	[SerializeField]
	private S13LocalAction switchOnAction;

	[SerializeField]
	private S13LocalAction switchOffAction;

	[SerializeField]
	private S13ObjectDynamic radioBroadcastObject;

	public bool IsPlaying => this.radioBroadcastObject.isPlaying;

	public void Play(AudioClip clip = null)
	{
		if (this.switchOnAction.IsExecutable)
		{
			this.switchOnAction.Execute();
		}
		if (clip != null)
		{
			S13HandlerDynamic s13HandlerDynamic;
			s13HandlerDynamic = S13Manager.GetGlobalHandler(this.radioBroadcastObject) as S13HandlerDynamic;
			if (s13HandlerDynamic != null)
			{
				s13HandlerDynamic.SetNextClip(clip);
				s13HandlerDynamic.Play(base.transform);
			}
			else
			{
				S13Debug.LogWarning("No handler found for radio object.", this);
			}
		}
		else
		{
			this.radioBroadcastObject.Play(base.transform);
		}
	}

	public void Stop()
	{
		if (this.switchOffAction.IsExecutable)
		{
			this.switchOffAction.Execute();
		}
		this.radioBroadcastObject.Stop();
	}
}
