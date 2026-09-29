using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : JMonoBehaviour
{
	[SerializeField]
	private VideoPlayer m_VideoPlayer;

	public bool IsPlaying { get; private set; }

	public event EventHandler OnComplete;

	private void Update()
	{
		if (this.IsPlaying && !(this.m_VideoPlayer == null) && !base.IsDisposed && !this.m_VideoPlayer.isPlaying)
		{
			this.IsPlaying = false;
			this.OnComplete.Send(this);
		}
	}

	public void SetCamera(Camera camera)
	{
		this.m_VideoPlayer.targetCamera = camera;
	}

	public void Play()
	{
		this.m_VideoPlayer.Play();
		this.IsPlaying = true;
	}

	public void Stop()
	{
		this.IsPlaying = false;
		this.m_VideoPlayer.Stop();
	}
}
