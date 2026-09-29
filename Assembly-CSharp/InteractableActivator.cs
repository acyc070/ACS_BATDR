using UnityEngine;

public class InteractableActivator : Interactable
{
	[SerializeField]
	private GameObject[] m_ActivationGameObjects;

	[SerializeField]
	private GameObject[] m_DeactivateGameObjects;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		unchecked
		{
			if (this.m_ActivationGameObjects != null)
			{
				for (int i = 0; i < this.m_ActivationGameObjects.Length; i++)
				{
					GameObject obj;
					obj = this.m_ActivationGameObjects[i];
					obj.SetActive(!obj.activeSelf);
				}
			}
			if (this.m_DeactivateGameObjects != null)
			{
				for (int j = 0; j < this.m_DeactivateGameObjects.Length; j++)
				{
					GameObject obj2;
					obj2 = this.m_DeactivateGameObjects[j];
					obj2.SetActive(!obj2.activeSelf);
				}
			}
		}
	}
}
