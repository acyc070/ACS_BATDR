using UnityEngine;

public class InteractableActiveSetter : InteractableInputDisplay
{
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		this.m_ActiveSetter.SetActive(active: true);
	}
}
