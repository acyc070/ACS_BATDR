public class InspectDataVO
{
	public string ActionLabel;

	public ArchivesInspectID ArchivesTitleID;

	public ArchivesInspectID ArchivesDescriptionID;

	public InspectDataVO(string actionLabel)
	{
		this.ActionLabel = actionLabel;
	}

	public InspectDataVO(ArchivesInspectID archivesTitleID, ArchivesInspectID archivesDescriptionID)
	{
		this.ArchivesTitleID = archivesTitleID;
		this.ArchivesDescriptionID = archivesDescriptionID;
	}
}
