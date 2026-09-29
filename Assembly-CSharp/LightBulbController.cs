using System;
using UnityEngine;

public class LightBulbController : JMonoBehaviour
{
	[SerializeField]
	private Light m_Light;

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private int m_MaterialIndex = 1;

	[SerializeField]
	private bool m_IsOn = true;

	public bool IsOn => this.m_Light.enabled;

	public event EventHandler OnTurnedOn;

	public event EventHandler OnTurnedOff;

	public override void Start()
	{
		if (!this.m_IsOn)
		{
			this.TurnOff();
		}
	}

	public void TurnOn()
	{
		this.m_LightRenderer.materials[this.m_MaterialIndex].SetInt("_Power", 1);
		this.m_Light.gameObject.SetActive(value: true);
		this.OnTurnedOn.Send(this);
	}

	public void TurnOff()
	{
		this.m_LightRenderer.materials[this.m_MaterialIndex].SetInt("_Power", 0);
		this.m_Light.gameObject.SetActive(value: false);
		this.OnTurnedOff.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTurnedOn = null;
		this.OnTurnedOff = null;
		base.OnDisposed();
	}
}
