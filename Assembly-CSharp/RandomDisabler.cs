using UnityEngine;

public class RandomDisabler : JMonoBehaviour
{
	private bool m_IsSet;

	public override void Awake()
	{
		if (!this.m_IsSet)
		{
			this.m_IsSet = true;
			if (Random.value < 0.2f)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
