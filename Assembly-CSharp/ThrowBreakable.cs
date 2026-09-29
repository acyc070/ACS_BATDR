using UnityEngine;

public class ThrowBreakable : MonoBehaviour
{
	[SerializeField]
	private GameObject m_Prefab;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private void OnTriggerEnter(Collider other)
	{
		ThrowObject component;
		component = other.GetComponent<ThrowObject>();
		if (component == null)
		{
			component = other.transform.root.GetComponent<ThrowObject>();
		}
		if (component != null)
		{
			this.m_Prefab.SetActive(value: true);
			this.m_Prefab.transform.SetParent(null);
			if (this.m_ActiveSetter != null)
			{
				this.m_ActiveSetter.SetActive(active: true);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
