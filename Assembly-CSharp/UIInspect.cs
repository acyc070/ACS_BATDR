using S13Audio.BATDR;
using TMPro;
using UnityEngine;

public class UIInspect : UIController
{
	[SerializeField]
	private TextMeshProUGUI m_ActionLabel;

	[Header("Archives")]
	[SerializeField]
	private GameObject m_ArchivesBG;

	[SerializeField]
	private TextMeshProUGUI m_ArchivesTitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_ArchivesDescriptionLabel;

	private InspectDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.Show(_data);
	}

	public void Show(object _data)
	{
		this.m_DataVO = (InspectDataVO)_data;
		BATDRUIAudioManager.UIInteractPromptBegin();
		if (this.m_DataVO.ActionLabel != null)
		{
			this.m_ArchivesTitleLabel.gameObject.SetActive(value: false);
			this.m_ArchivesDescriptionLabel.gameObject.SetActive(value: false);
			this.m_ArchivesBG.SetActive(value: false);
			this.m_ActionLabel.text = this.m_DataVO.ActionLabel;
		}
		else
		{
			this.m_ActionLabel.gameObject.SetActive(value: false);
			if (this.m_DataVO.ArchivesTitleID != ArchivesInspectID.NONE)
			{
				this.m_ArchivesTitleLabel.text = TextUtility.GetKey(this.m_DataVO.ArchivesTitleID.ToString()).ToUpper();
			}
			else
			{
				this.m_ArchivesTitleLabel.gameObject.SetActive(value: false);
			}
			if (this.m_DataVO.ArchivesDescriptionID != ArchivesInspectID.NONE)
			{
				this.m_ArchivesDescriptionLabel.text = TextUtility.GetKey(this.m_DataVO.ArchivesDescriptionID.ToString());
			}
			else
			{
				this.m_ArchivesBG.SetActive(value: false);
				this.m_ArchivesDescriptionLabel.gameObject.SetActive(value: false);
			}
		}
		base.m_Visuals.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		if (!(base.m_Visuals == null))
		{
			base.m_Visuals.gameObject.SetActive(value: false);
			BATDRUIAudioManager.UIInteractPromptEnd();
		}
	}

	public void Clear()
	{
		this.m_ActionLabel.text = "";
		this.Hide();
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
