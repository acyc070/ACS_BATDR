using System;
using UnityEngine;

[Serializable]
public class CameraFOV : JDisposable
{
	[SerializeField]
	private bool m_Active = true;

	[SerializeField]
	private float m_BaseFOV = 55f;

	[SerializeField]
	private float m_RunFOV = 60f;

	[SerializeField]
	private float m_TransitionSpeed = 0.5f;

	private Camera[] m_Cameras;

	public float FOV
	{
		get
		{
			if (this.m_Cameras != null && this.m_Cameras.Length != 0)
			{
				return this.m_Cameras[0].fieldOfView;
			}
			return 0f;
		}
	}

	public void Init(params Camera[] cameras)
	{
		if (this.m_Active)
		{
			this.m_Cameras = cameras;
			this.SetFOV(this.m_BaseFOV);
		}
	}

	public void UpdateVOD(bool isRunning)
	{
		if (!this.m_Active)
		{
			return;
		}
		if (isRunning)
		{
			if (!(this.FOV >= this.m_RunFOV))
			{
				this.SetFOV(this.DORun());
			}
		}
		else if (!(this.FOV <= this.m_BaseFOV))
		{
			this.SetFOV(this.DOBase());
		}
	}

	private float DORun()
	{
		return this.FOV + this.m_TransitionSpeed;
	}

	private float DOBase()
	{
		return this.FOV - this.m_TransitionSpeed;
	}

	public void SetFOV(float value)
	{
		for (int i = 0; i < this.m_Cameras.Length; i = unchecked(i + 1))
		{
			if (this.m_Cameras[i] != null)
			{
				this.m_Cameras[i].fieldOfView = value;
			}
		}
	}

	public void SetActiveFOV(bool active)
	{
		this.m_Active = active;
	}

	public void ResetFOV()
	{
		this.SetFOV(this.m_BaseFOV);
	}

	protected override void OnDisposed()
	{
		this.m_Cameras = null;
		base.OnDisposed();
	}
}
