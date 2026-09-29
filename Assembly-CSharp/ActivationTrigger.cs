using UnityEngine;

public class ActivationTrigger : EventTrigger
{
	[SerializeField]
	private GameObject[] m_ActivationObjects;

	protected override void OnInternalEnter(Collider col)
	{
		for (int i = 0; i < this.m_ActivationObjects.Length; i = unchecked(i + 1))
		{
			GameObject obj;
			obj = this.m_ActivationObjects[i];
			obj.SetActive(!obj.activeSelf);
		}
	}
}
