using UnityEngine;

public class JAudioListener : JMonoBehaviour
{
	[SerializeField]
	private AudioListener m_AudioListener;

	public AudioListener AudioListener => this.m_AudioListener;

	public override void Start()
	{
	}

	public override void OnEnable()
	{
		if (GameManager.Instance.UIManager != null)
		{
			GameManager.Instance.UIManager.SetAudioListener(active: false);
		}
		if ((bool)GameManager.Instance.AudioListener)
		{
			GameManager.Instance.AudioListener.AudioListener.enabled = false;
		}
		GameManager.Instance.AudioListener = this;
		GameManager.Instance.AudioListener.enabled = true;
	}

	public override void OnDisable()
	{
		this.Disable();
	}

	private void Disable()
	{
		if (GameManager.Instance.AudioListener == this || GameManager.Instance.AudioListener == null)
		{
			if (GameManager.Instance.UIManager != null)
			{
				GameManager.Instance.UIManager.SetAudioListener(active: true);
			}
			GameManager.Instance.AudioListener = null;
		}
	}

	protected override void OnDisposed()
	{
		this.Disable();
		base.OnDisposed();
	}
}
