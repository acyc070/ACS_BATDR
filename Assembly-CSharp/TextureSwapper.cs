using System;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
	[Serializable]
	public class Emotion
	{
		public string name;

		public Texture[] textureFrames;

		public int currentFrame;

		public bool isReverse;
	}

	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private HeadTracker m_HeadTracker;

	[SerializeField]
	private Emotion[] m_Emotions;

	private bool blink;

	private bool sad;

	private bool close;

	private bool open;

	private bool cry;

	private bool shock;

	private void Update()
	{
		if (this.blink)
		{
			this.DoEmotion("blink", ref this.blink);
		}
		if (this.sad)
		{
			this.DoEmotion("sad", ref this.sad);
		}
		if (this.close)
		{
			this.DoEmotion("close", ref this.close);
		}
		if (this.open)
		{
			this.DoEmotion("open", ref this.open);
		}
		if (this.cry)
		{
			this.DoEmotion("cry", ref this.cry);
		}
		if (this.shock)
		{
			this.DoEmotion("shock", ref this.shock);
		}
	}

	public void Shock()
	{
		this.open = false;
		this.close = false;
		this.sad = false;
		this.cry = false;
		this.blink = false;
		this.shock = true;
	}

	public void Blink()
	{
		this.open = false;
		this.close = false;
		this.sad = false;
		this.cry = false;
		this.shock = false;
		this.blink = true;
	}

	public void Sad()
	{
		this.open = false;
		this.close = false;
		this.blink = false;
		this.cry = false;
		this.shock = false;
		this.sad = true;
	}

	public void Open()
	{
		this.close = false;
		this.blink = false;
		this.sad = false;
		this.cry = false;
		this.shock = false;
		this.open = true;
	}

	public void Close()
	{
		this.open = false;
		this.blink = false;
		this.sad = false;
		this.cry = false;
		this.shock = false;
		this.close = true;
	}

	public void Cry()
	{
		this.open = false;
		this.blink = false;
		this.sad = false;
		this.close = false;
		this.shock = false;
		this.cry = true;
	}

	public void DisableHeadTrack()
	{
		if (this.m_HeadTracker != null)
		{
			this.m_HeadTracker.SetIgnore(ignore: true);
		}
	}

	public void EnableHeadTrack()
	{
		if (this.m_HeadTracker != null)
		{
			this.m_HeadTracker.SetIgnore(ignore: false);
		}
	}

	private void DoEmotion(string emotionType, ref bool boolType)
	{
		Emotion[] emotions;
		emotions = this.m_Emotions;
		unchecked
		{
			foreach (Emotion emotion in emotions)
			{
				if (!(emotion.name.ToLower() == emotionType))
				{
					continue;
				}
				for (int j = 0; j < this.m_MeshRenderer.materials.Length; j++)
				{
					if (emotion.currentFrame >= emotion.textureFrames.Length)
					{
						emotion.isReverse = true;
					}
					if (emotion.isReverse)
					{
						emotion.currentFrame--;
					}
					if (emotion.currentFrame < 0)
					{
						emotion.currentFrame = 0;
						boolType = false;
						emotion.isReverse = false;
					}
					this.m_MeshRenderer.materials[j].mainTexture = emotion.textureFrames[emotion.currentFrame];
					if (!emotion.isReverse & boolType)
					{
						emotion.currentFrame++;
					}
				}
			}
		}
	}
}
