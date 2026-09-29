using System;
using UnityEngine;

[Serializable]
public class SaveFileData : IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_FileID;

	[SerializeField]
	private string m_FileName;

	[SerializeField]
	private string m_FileTitle;

	[SerializeField]
	private string m_TimeStamp;

	[SerializeField]
	private string m_CurrentZone;

	[SerializeField]
	private SectionID m_CurrentSectionID;

	[SerializeField]
	private DifficultyLevel m_Difficulty;

	public int ID => this.m_FileID;

	public string Name => this.m_FileName;

	public string Title => this.m_FileTitle;

	public string TimeStamp => this.m_TimeStamp;

	public string CurrentZone => this.m_CurrentZone;

	public SectionID CurrentSectionID => this.m_CurrentSectionID;

	public DifficultyLevel Difficulty => this.m_Difficulty;

	public SaveFileData(int id, string fileName, string fileTitle)
	{
		this.m_FileID = id;
		this.m_FileName = fileName;
		this.m_FileTitle = fileTitle;
	}

	public void Update(SaveData saveData)
	{
		if (GameManager.Instance.Player != null)
		{
			this.m_CurrentZone = GameManager.Instance.Player.CurrentZone;
			this.m_CurrentSectionID = GameManager.Instance.Player.CurrentSectionID;
		}
		this.m_TimeStamp = new DateTime(saveData.TimeData.Date).ToString();
		this.m_Difficulty = saveData.Difficulty.Difficulty;
	}
}
