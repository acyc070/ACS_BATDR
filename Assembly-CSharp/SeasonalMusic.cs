using S13Audio;
using UnityEngine;

public class SeasonalMusic : JMonoBehaviour
{
	[SerializeField]
	private S13AudioHandler m_Music;

	[SerializeField]
	private bool m_RequiresPlayer = true;

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		S13Handler handler;
		handler = this.m_Music.GetHandler();
		if (handler == null)
		{
			return;
		}
		if (this.m_RequiresPlayer)
		{
			if (GameManager.Instance.Player.gameObject.activeInHierarchy)
			{
				if (!handler.isPlaying)
				{
					this.m_Music.Play();
				}
			}
			else if (handler.isPlaying)
			{
				this.m_Music.Stop();
			}
		}
		else if (!handler.isPlaying)
		{
			this.m_Music.Play();
		}
	}
}
