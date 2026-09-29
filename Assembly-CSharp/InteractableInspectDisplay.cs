using UnityEngine;

public class InteractableInspectDisplay : Interactable
{
	[Header("Visual Options")]
	[SerializeField]
	protected bool m_IsReal;

	[Header("Input Options")]
	[SerializeField]
	protected string m_ActionMessage;

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.ShowInspect(this.m_ActionMessage, this.m_IsReal);
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInspect();
	}

	protected override bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return false;
	}
}
