using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Thunder : JMonoBehaviour
{
	[Serializable]
	public class ThunderGroup
	{
		public AudioSource AudioSource;

		public Light[] Light;

		[HideInInspector]
		public List<float> Intensity = new List<float>();
	}

	[SerializeField]
	private bool m_ActiveOnStart;

	[SerializeField]
	private AudioClip m_AudioClipThunder;

	[SerializeField]
	private float m_TimeMin = 60f;

	[SerializeField]
	private float m_TimeMax = 240f;

	[Header("Thunder Groups")]
	[SerializeField]
	private ThunderGroup[] m_ThunderGroups;

	private float m_Timer;

	private float m_TimeLimit;

	private bool m_Thunder;

	private bool m_IsActive;

	public override void Start()
	{
		unchecked
		{
			for (int i = 0; i < this.m_ThunderGroups.Length; i++)
			{
				ThunderGroup thunderGroup;
				thunderGroup = this.m_ThunderGroups[i];
				for (int j = 0; j < thunderGroup.Light.Length; j++)
				{
					Light light;
					light = thunderGroup.Light[j];
					thunderGroup.Intensity.Add(light.intensity);
				}
			}
			this.m_TimeLimit = global::UnityEngine.Random.Range(this.m_TimeMin, this.m_TimeMax);
			if (this.m_ActiveOnStart)
			{
				this.m_IsActive = true;
			}
		}
	}

	private void Update()
	{
		if (!this.m_IsActive || this.m_Thunder)
		{
			return;
		}
		this.m_Timer += Time.deltaTime;
		if (!(this.m_Timer > this.m_TimeLimit))
		{
			return;
		}
		this.m_Thunder = true;
		Transform transform;
		transform = null;
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		unchecked
		{
			for (int i = 0; i < this.m_ThunderGroups.Length; i++)
			{
				ThunderGroup thunderGroup;
				thunderGroup = this.m_ThunderGroups[i];
				thunderGroup.AudioSource.PlayOneShot(this.m_AudioClipThunder);
				float num2;
				num2 = Vector3.Distance(thunderGroup.AudioSource.transform.position, position);
				if (num2 < num)
				{
					transform = thunderGroup.AudioSource.transform;
					num = num2;
				}
				for (int j = 0; j < thunderGroup.Light.Length; j++)
				{
					Light obj;
					obj = thunderGroup.Light[j];
					obj.intensity = 15f;
					obj.DOKill();
					obj.DOIntensity(thunderGroup.Intensity[j], 0.5f).OnComplete(this.ResetThunder);
				}
			}
			if (transform != null && Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < 20f)
			{
				CameraEffects.ShakeRotation(1f, 0.5f);
			}
		}
	}

	private void ResetThunder()
	{
		this.m_Timer = 0f;
		this.m_TimeLimit = global::UnityEngine.Random.Range(this.m_TimeMin, this.m_TimeMax);
		this.m_Thunder = false;
	}

	public void ForceThunder()
	{
		this.m_IsActive = true;
		this.m_Thunder = false;
		this.m_TimeLimit = 0f;
		this.m_Timer = 10000f;
	}

	public void StopThunder()
	{
		this.m_IsActive = false;
		this.m_Thunder = true;
		this.m_Timer = -1f;
	}

	public void StopAll()
	{
		this.StopThunder();
		unchecked
		{
			for (int i = 0; i < this.m_ThunderGroups.Length; i++)
			{
				ThunderGroup thunderGroup;
				thunderGroup = this.m_ThunderGroups[i];
				thunderGroup.AudioSource.Stop();
				thunderGroup.AudioSource.clip = null;
				thunderGroup.AudioSource.volume = 0f;
				for (int j = 0; j < thunderGroup.Light.Length; j++)
				{
					thunderGroup.Light[j].DOKill();
					thunderGroup.Light[j].intensity = thunderGroup.Intensity[j];
				}
			}
		}
	}
}
