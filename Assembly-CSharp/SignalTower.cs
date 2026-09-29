using System;
using UnityEngine;

public class SignalTower : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private Transform m_Lighting;

	public event EventHandler On;

	public event EventHandler Off;

	public override void Awake()
	{
		if (!this.m_IsActive)
		{
			this.TurnOff();
		}
	}

	public void TurnOn()
	{
		this.SetLighting(active: true);
		this.On.Send(this);
	}

	public void TurnOff()
	{
		this.SetLighting(active: false);
		this.Off.Send(this);
	}

	private void SetLighting(bool active)
	{
		this.m_Lighting.gameObject.SetActive(active);
	}
}
