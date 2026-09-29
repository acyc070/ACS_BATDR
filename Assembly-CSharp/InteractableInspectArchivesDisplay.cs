using UnityEngine;

public class InteractableInspectArchivesDisplay : Interactable
{
	[Header("Inspect Display")]
	[SerializeField]
	protected ArchivesInspectID m_ArchivesTitleID;

	[SerializeField]
	protected ArchivesInspectID m_ArchivesDescriptionID;

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.ShowInspectArchives(this.m_ArchivesTitleID, this.m_ArchivesDescriptionID);
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
