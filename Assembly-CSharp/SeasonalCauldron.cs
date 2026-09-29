using UnityEngine;

public class SeasonalCauldron : ActionEventController<SeasonalCauldronContent, SeasonalCauldronData>
{
	[SerializeField]
	private ParticleSystem[] m_Particles;

	[SerializeField]
	private GameObject m_Deposit;

	public void EnableParticles()
	{
		for (int i = 0; i < this.m_Particles.Length; i = unchecked(i + 1))
		{
			this.m_Particles[i].Play();
		}
	}

	public void DisableParticles()
	{
		for (int i = 0; i < this.m_Particles.Length; i = unchecked(i + 1))
		{
			this.m_Particles[i].Stop();
		}
	}

	public void Deposit()
	{
		this.m_Deposit.gameObject.SetActive(value: true);
		base.Invoke("DepositComplete", 10f);
	}

	private void DepositComplete()
	{
		this.m_Deposit.gameObject.SetActive(value: false);
	}
}
