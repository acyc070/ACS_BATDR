using UnityEngine;

public class BulletLine : JComponent
{
	[SerializeField]
	private GameObject m_Content;

	[SerializeField]
	private GameObject m_Muzzle;

	public override void Awake()
	{
		this.SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		this.m_Content.SetActive(active);
		this.m_Muzzle.SetActive(active);
	}

	public void Enable(float delay)
	{
		base.Invoke("InvokeEnable", delay);
	}

	private void InvokeEnable()
	{
		this.SetActive(active: true);
		this.Disable(0.015f);
	}

	public void Disable(float delay)
	{
		base.Invoke("InvokeDisable", delay);
	}

	private void InvokeDisable()
	{
		this.SetActive(active: false);
	}
}
