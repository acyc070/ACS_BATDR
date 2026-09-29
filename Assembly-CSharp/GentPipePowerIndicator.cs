using UnityEngine;

public class GentPipePowerIndicator : JMonoBehaviour
{
	[SerializeField]
	private GameObject[] m_PowerLevels;

	public int Power { get; private set; }

	public void SetPowerLevel(int level)
	{
		this.Power = level;
		if (this.Power >= this.m_PowerLevels.Length)
		{
			this.Power = this.m_PowerLevels.Length;
		}
		else if (this.Power <= 0)
		{
			this.Power = 0;
		}
		for (int i = 0; i < this.m_PowerLevels.Length; i = unchecked(i + 1))
		{
			GameObject gameObject;
			gameObject = this.m_PowerLevels[i];
			if (i >= level)
			{
				gameObject.SetActive(value: false);
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	public void Initialize(GameObject[] powerLevels)
	{
		this.m_PowerLevels = powerLevels;
	}
}
