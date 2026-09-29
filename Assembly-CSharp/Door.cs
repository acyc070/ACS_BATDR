using S13Audio;
using UnityEngine;

public class Door : ActionEventController<DoorContent, DoorData>
{
	[Header("Section Identifier")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Door Identifier")]
	[SerializeField]
	private DoorID m_ID;

	public SectionID SectionID => this.m_SectionID;

	public DoorID ID => this.m_ID;

	public S13SimpleCloseablePortal S13CloseablePortal { get; private set; }

	protected override void OnInitialized()
	{
		this.S13CloseablePortal = base.Content.transform.GetComponentInChildren<S13SimpleCloseablePortal>(includeInactive: true);
	}
}
