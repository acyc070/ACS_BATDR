using S13Audio.BATDR;
using TMPro;
using UnityEngine;

public class UIInteractionInvalid : UIController
{
	[SerializeField]
	private RectTransform m_BGScale;

	[SerializeField]
	private TextMeshProUGUI m_ActionLabel;

	private InspectDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.Show(_data);
	}

	public void Show(object _data)
	{
		this.m_DataVO = (InspectDataVO)_data;
		if (this.m_DataVO == null)
		{
			Debug.LogError("UIInteractionInvalid :: OnInitialized => m_DataVO = " + this.m_DataVO, this);
		}
		BATDRUIAudioManager.UIInteractPromptBegin();
		this.m_ActionLabel.text = this.m_DataVO.ActionLabel;
		this.m_ActionLabel.ForceMeshUpdate();
		Vector2 sizeDelta;
		sizeDelta = this.m_BGScale.sizeDelta;
		sizeDelta.x = this.m_ActionLabel.renderedWidth;
		this.m_BGScale.sizeDelta = sizeDelta;
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
